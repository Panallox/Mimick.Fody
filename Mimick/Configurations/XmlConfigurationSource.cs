using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Mimick.Configurations
{
    /// <summary>
    /// A configuration source class which loads values from an XML document.
    /// </summary>
    public sealed class XmlConfigurationSource : IConfigurationSource
    {
        private readonly ReaderWriterLockSlim sync;

        private XmlDocument document;
        private FileInfo path;
        private XmlSource source;
        private Stream stream;
        private long streamPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        public XmlConfigurationSource(XmlDocument doc)
        {
            document = doc ?? throw new ArgumentNullException(nameof(doc));
            source = XmlSource.Document;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
        /// </summary>
        /// <param name="filename">The full path to the document.</param>
        public XmlConfigurationSource(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            path = new FileInfo(filename);
            source = XmlSource.File;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            if (!path.Exists)
                throw new FileNotFoundException($"Cannot resolve an XML configuration source at path '{filename}'");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        public XmlConfigurationSource(Stream src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (!src.CanRead)
                throw new IOException("Cannot read content from the provided stream");

            source = XmlSource.Stream;
            stream = src;
            streamPosition = src.Position;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="XmlConfigurationSource"/> class.
        /// </summary>
        ~XmlConfigurationSource() => Dispose(false);

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
            if (disposing && source == XmlSource.Stream && stream != null)
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
                    case XmlSource.File:
                        document = new XmlDocument();
                        document.Load(path.FullName);
                        break;

                    case XmlSource.Stream:
                        document = new XmlDocument();
                        document.Load(stream);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Cannot load a {source.ToString().ToLower()} XML document", ex);
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
                case XmlSource.File:
                    Load();
                    break;

                case XmlSource.Stream:
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
                var node = document.SelectSingleNode(name);

                if (node == null)
                    return null;

                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        return node.Value;
                    case XmlNodeType.Element:
                        return node.InnerText;
                    case XmlNodeType.Text:
                        return node.Value;
                    default:
                        return null;
                }
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
        /// Indicates the source of an XML configuration source.
        /// </summary>
        private enum XmlSource
        {
            /// <summary>
            /// The configuration source was provided a concrete XML document.
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
