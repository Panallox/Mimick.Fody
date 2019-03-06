using System;
using System.Linq;
using System.IO;
using System.Threading;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;

namespace Mimick
{
    /// <summary>
    /// A configuration source class which loads values from a YAML document.
    /// </summary>
    public sealed class YamlConfigurationSource : IConfigurationSource
    {
        private readonly ReaderWriterLockSlim sync;

        private YamlDocument document;
        private FileInfo path;
        private YamlSource source;
        private Stream stream;
        private long streamPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlConfigurationSource"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        public YamlConfigurationSource(YamlDocument doc)
        {
            document = doc ?? throw new ArgumentNullException(nameof(doc));
            source = YamlSource.Document;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlConfigurationSource"/> class.
        /// </summary>
        /// <param name="filename">The full path to the document.</param>
        public YamlConfigurationSource(string filename)
        {
            path = new FileInfo(filename ?? throw new ArgumentNullException(nameof(filename)));
            source = YamlSource.File;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlConfigurationSource"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        public YamlConfigurationSource(Stream src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (!src.CanRead)
                throw new IOException("Cannot read content from the provided stream");

            source = YamlSource.Stream;
            stream = src;
            streamPosition = src.Position;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="YamlConfigurationSource"/> class.
        /// </summary>
        ~YamlConfigurationSource() => Dispose(false);
        
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
            if (disposing && source == YamlSource.Stream && stream != null)
            {
                try { stream.Dispose(); }
                catch { }
            }
        }

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        public void Load()
        {
            sync.EnterWriteLock();

            try
            {
                switch (source)
                {
                    case YamlSource.File:
                        using (var reader = new StreamReader(path.FullName))
                        using (var content = new StringReader(reader.ReadToEnd()))
                        {
                            var yaml = new YamlStream();
                            yaml.Load(content);
                            document = yaml.Documents[0];
                        }
                        break;

                    case YamlSource.Stream:
                        using (var reader = new StreamReader(stream))
                        using (var content = new StringReader(reader.ReadToEnd()))
                        {
                            var yaml = new YamlStream();
                            yaml.Load(content);
                            document = yaml.Documents[0];
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Cannot load a {source.ToString().ToLower()} YAML document", ex);
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
                case YamlSource.File:
                    Load();
                    break;

                case YamlSource.Stream:
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
                var current = document.RootNode;

                foreach (var part in parts)
                {
                    try
                    {
                        var key = new YamlScalarNode(part);
                        var node = current[key];

                        if (node == null)
                            return null;

                        current = node;
                    }
                    catch (KeyNotFoundException)
                    {
                        return null;
                    }
                }

                if (current.NodeType == YamlNodeType.Scalar)
                    return ((YamlScalarNode)current).Value;

                throw new ConfigurationException($"Cannot process the value of a YAML configuration", name);
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
        /// Indicates the source of a YAML configuration source.
        /// </summary>
        private enum YamlSource
        {
            /// <summary>
            /// The configuration source was provided a concrete YAML document.
            /// </summary>
            Document,

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
