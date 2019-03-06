using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mimick
{
    /// <summary>
    /// A configuration source class which loads values from a JSON document.
    /// </summary>
    public sealed class JsonConfigurationSource : IConfigurationSource
    {
        private readonly JsonSerializer serializer;
        private readonly ReaderWriterLockSlim sync;

        private JObject document;
        private FileInfo path;
        private JsonSource source;
        private Stream stream;
        private long streamPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurationSource"/> class.
        /// </summary>
        /// <param name="filename">The full path to the document.</param>
        public JsonConfigurationSource(string filename)
        {
            path = new FileInfo(filename ?? throw new ArgumentNullException(nameof(filename)));
            serializer = new JsonSerializer();
            source = JsonSource.File;
            sync = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurationSource"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        public JsonConfigurationSource(Stream src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (!src.CanRead)
                throw new IOException("Cannot read content from the provided stream");

            serializer = new JsonSerializer();
            source = JsonSource.Stream;
            stream = src;
            streamPosition = src.Position;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing && source == JsonSource.Stream && stream != null)
            {
                try { stream.Dispose(); }
                catch { }
            }
        }

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        /// <exception cref="ConfigurationException"></exception>
        public void Load()
        {
            sync.EnterWriteLock();

            try
            {
                switch (source)
                {
                    case JsonSource.File:
                        using (var reader = new StreamReader(path.FullName))
                        using (var content = new JsonTextReader(reader))
                        {
                            document = serializer.Deserialize<JObject>(content);
                        }
                        break;

                    case JsonSource.Stream:
                        using (var reader = new StreamReader(stream))
                        using (var content = new JsonTextReader(reader))
                        {
                            document = serializer.Deserialize<JObject>(content);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Cannot load a {source.ToString().ToLower()} JSON document", ex);
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        public void Refresh()
        {
            switch (source)
            {
                case JsonSource.File:
                    Load();
                    break;

                case JsonSource.Stream:
                    if (stream.CanSeek)
                    {
                        stream.Seek(streamPosition, SeekOrigin.Begin);
                        Load();
                    }
                    break;
            }
        }

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>
        /// The configuration value; otherwise, <c>null</c> if the configuration could not be found.
        /// </returns>
        public string Resolve(string name)
        {
            sync.EnterReadLock();

            try
            {
                var parts = name.Split('.');
                var current = (JToken)document;

                foreach (var part in parts)
                {
                    if (current.Type != JTokenType.Object)
                        return null;

                    current = current[part];

                    if (current == null)
                        return null;
                }

                return current.Value<string>();
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns>
        ///   <c>true</c> if the configuration is resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryResolve(string name, out string value) => (value = Resolve(name)) != null;

        /// <summary>
        /// Indicates the source of a JSON configuration source.
        /// </summary>
        private enum JsonSource
        {
            /// <summary>
            /// The configuration source was provided a file path.
            /// </summary>
            File,

            /// <summary>
            /// The configuration source was provided a stream.
            /// </summary>
            Stream
        }
    }
}
