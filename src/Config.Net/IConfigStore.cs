using System;

namespace Config.Net {
    /// <summary>
    /// Configuration store interface
    /// </summary>
    public interface IConfigStore : IDisposable {
        /// <summary>
        /// Returns true if store supports read operation.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Returns true if store supports write operation.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Reads a key from the store.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>If key exists in the store returns the value, othwise returns null.</returns>
        string? Read(string key);

        /// <summary>
        /// Writes a key to the store.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Key value. Value of NULL usually means the key will be deleted, at least
        /// this is the recomendation for the custom store implementers.</param>
        void Write(string key, string? value);
    }
}