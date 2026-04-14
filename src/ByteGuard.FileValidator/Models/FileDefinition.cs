namespace ByteGuard.FileValidator.Models
{
    /// <summary>
    /// Definition of filetype, their valid signatures, and potentially their valid subtype signatures.
    /// </summary>
    internal class FileDefinition
    {
        /// <summary>
        /// File type in question (e.g. .jpw, .png, .pdf, etc).
        /// </summary>
        public string FileType { get; set; } = default!;

        /// <summary>
        /// Whether this file type is allowed to omit a binary signature. Defaults to <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Some file types, such as .txt files, do not have a binary signature.
        /// Setting this property to <c>true</c> allows files of this type to be considered valid even if they do not have a binary signature.
        /// This is useful for file types that do not have a binary signature, but still need to be validated by other mechanisms.
        /// </remarks>
        public bool AllowMissingSignature { get; set; } = false;

        /// <summary>
        /// Valid header signatures.
        /// </summary>
        /// <remarks>
        /// Multiple signatures can be specified for file types that have more than one valid signature.
        /// </remarks>
        public List<byte[]> ValidSignatures { get; set; } = new List<byte[]>();

        /// <summary>
        /// Potential offset for the signature. Defaults to <c>0</c>.
        /// </summary>
        /// <remarks>
        /// Some file types have their signature located at a specific offset, e.g. MP4 files have their signature at offset 4.
        /// </remarks>
        public int SignatureOffset { get; set; }

        /// <summary>
        /// Whether the file type has a subtype that also needs to be validated. Defaults to <c>false</c>.
        /// </summary>
        public bool HasSubtype { get; set; }

        /// <summary>
        /// Offset for the subtype signature. Defaults to <c>4</c>.
        /// </summary>
        public int SubtypeOffset { get; set; } = 4;

        /// <summary>
        /// Valid subtype signatures.
        /// </summary>
        /// <remarks>
        /// Multiple signatures can be specified for file types that have more than one valid signature.
        /// </remarks>
        public List<byte[]> ValidSubtypeSignatures { get; set; } = new List<byte[]>();
    }
}
