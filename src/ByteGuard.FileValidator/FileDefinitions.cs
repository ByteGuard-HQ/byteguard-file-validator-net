using ByteGuard.FileValidator.Models;

namespace ByteGuard.FileValidator;

internal static class FileDefinitions
{
    /// <summary>
    /// List of all supported valid file definitions, incl. their header signatures (magic numbers)
    /// and potentially their corresponding valid subtype signatures.
    /// </summary>
    internal static readonly List<FileDefinition> SupportedFileDefinitions = new List<FileDefinition>
    {
        new FileDefinition
        {
            FileType = FileExtensions.Jpeg,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Jpg,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Jpe,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Pdf,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D } // %PDF-
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Png,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } // ‰PNG␍␊␚␊
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Bmp,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x42, 0x4D } // BM
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Doc,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } // ÐÏ␑à¡±␚á
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Docx,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .docx.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Odp,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .odp.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Ods,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .ods.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Odt,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .odt.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Rtf,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 } // {\rtf1
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Xlsx,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .xlsx.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Xls,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } // ÐÏ␑à¡±␚á
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Pptx,
            // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .pptx.
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.M4a,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
            },
            SignatureOffset = 4,
            HasSubtype = true,
            SubtypeOffset = 8,
            ValidSubtypeSignatures = new List<byte[]>
            {
                new byte[] { 0x4D, 0x34, 0x41, 0x20 } // M4A_
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Mov,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
            },
            SignatureOffset = 4,
            HasSubtype = true,
            SubtypeOffset = 8,
            ValidSubtypeSignatures = new List<byte[]>
            {
                new byte[] { 0x71, 0x74, 0x20, 0x20 } // qt__ (Quicktime)
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Avi,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF
            },
            HasSubtype = true,
            SubtypeOffset = 8,
            ValidSubtypeSignatures = new List<byte[]>
            {
                new byte[] { 0x41, 0x56, 0x49, 0x20 } // AVI_
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Mp3,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0xFF, 0xFB }, // Without ID3 tag
                new byte[] { 0xFF, 0xF2 }, // Without ID3 tag
                new byte[] { 0xFF, 0xF3 }, // Without ID3 tag
                new byte[] { 0x49, 0x44, 0x33 } // ID3
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Mp4,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
            },
            SignatureOffset = 4,
            HasSubtype = true,
            SubtypeOffset = 8,
            ValidSubtypeSignatures = new List<byte[]>
            {
                new byte[] { 0x6D, 0x6D, 0x70, 0x34 }, // mmp4 (MP4)
                new byte[] { 0x6D, 0x70, 0x34, 0x32 }, // mp42 (MP4 v2)
                new byte[] { 0x69, 0x73, 0x6F, 0x6D }, // isom (ISO Base Media File)
                new byte[] { 0x4D, 0x53, 0x4E, 0x56 } // MSNV (MPEG-4)
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Wav,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF
            },
            HasSubtype = true,
            SubtypeOffset = 8,
            ValidSubtypeSignatures = new List<byte[]>
            {
                new byte[] { 0x57, 0x41, 0x56, 0x45 } // WAVE
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Txt,
            AllowMissingSignature = true
        },
        new FileDefinition
        {
            FileType = FileExtensions.Csv,
            AllowMissingSignature = true
        },
        new FileDefinition
        {
            FileType = FileExtensions.Ico,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x00, 0x00, 0x01, 0x00 }, // ␀␀␁␀
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Heic,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63 } // ftypheic
            },
            SignatureOffset = 4
        },
        new FileDefinition
        {
            FileType = FileExtensions.Gif,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x47, 0x49, 0x46, 0x38 }, // GIF8
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Tif,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x49, 0x49, 0x2A, 0x00 }, // II*␀ (Intel byte order)
                new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, // MM␀* (Motorola byte order)
                new byte[] { 0x49, 0x49, 0x2B, 0x00 }, // II+␀ (BigTIFF, Intel byte order)
                new byte[] { 0x4D, 0x4D, 0x00, 0x2B }  // MM␀+ (BigTIFF, Motorola byte order)
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Tiff,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x49, 0x49, 0x2A, 0x00 }, // II*␀ (Intel byte order)
                new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, // MM␀* (Motorola byte order)
                new byte[] { 0x49, 0x49, 0x2B, 0x00 }, // II+␀ (BigTIFF, Intel byte order)
                new byte[] { 0x4D, 0x4D, 0x00, 0x2B }  // MM␀+ (BigTIFF, Motorola byte order)
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Ogg,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x4F, 0x67, 0x67, 0x53 } // OggS
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Oga,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x4F, 0x67, 0x67, 0x53 } // OggS
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Ogv,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x4F, 0x67, 0x67, 0x53 } // OggS
            }
        },
        new FileDefinition
        {
            FileType = FileExtensions.Ogx,
            ValidSignatures = new List<byte[]>
            {
                new byte[] { 0x4F, 0x67, 0x67, 0x53 } // OggS
            }
        }
    };
}
