using System;
using System.Collections.Generic;
using System.Linq;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.encryption;
using org.apache.pdfbox.util;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace scSearchContrib.Crawler.Utilities
{
    /// <summary>
    /// The PDF utilities.
    /// </summary>
    public class PdfUtil
    {
        public const string PdfMimetype = "application/pdf";

        /// <summary>
        /// Parses a pdf item and returns its content as string.
        /// </summary>
        /// <param name="mediaItem">MediaItem (should be a pdf - otherwise an empty string will be returned).</param>
        /// <returns>String represantation of the pdf content.</returns>
        public static string ParsePdf(MediaItem mediaItem)
        {
            if (mediaItem.MimeType != PdfMimetype) return string.Empty;

            PDDocument doc = null;
            ikvm.io.InputStreamWrapper wrapper = null;

            try
            {
                var stream = mediaItem.GetMediaStream();
                wrapper = new ikvm.io.InputStreamWrapper(stream);
                doc = PDDocument.load(wrapper);

                if (doc.isEncrypted())
                {
                    string[] pwArray = LoadPasswords();

                    doc = Decrypt(doc, pwArray);
                    if (doc == null)
                    {
                        Log.Warn("PdfUtil :: ParsePDF :: Decryption Failed for: [" + mediaItem.Name + "]", typeof(PdfUtil));
                        return string.Empty;
                    }
                    else
                    {
                        Log.Debug("PdfUtil :: ParsePDF :: Successfully decrypted [" + mediaItem.Name + "]", typeof(PdfUtil));
                    }
                }

                var stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            catch (Exception ex)
            {
                Log.Error("PdfUtil :: ParsePDF :: Error parsing pdf: [" + mediaItem.Name + "]", ex);
                return string.Empty;
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                    wrapper.close();
                }
            }
        }

        /// <summary>
        /// Loads the passwords from the configuration file (Key: PdfUtil.Security.Passwords).
        /// The passwords must be a pipe separated list.
        /// </summary>
        /// <returns>String array with all available passwords.</returns>
        private static string[] LoadPasswords()
        {
            var pws = Settings.GetSetting("PdfUtil.Security.Passwords");
            return Sitecore.StringUtil.Split(pws, '|', false);
        }

        /// <summary>
        /// Tries to decrypt a document with the given passwords.
        /// </summary>
        /// <param name="doc">Document of type PDDocument.</param>
        /// <param name="passwords">Passwords of type string array.</param>
        /// <returns>Decrypted document (PDDocument) or null if decryption fails.</returns>
        private static PDDocument Decrypt(PDDocument doc, ICollection<string> passwords)
        {
            if (!passwords.Any())
            {
                throw new ApplicationException("PDfUtil :: Decrypt :: supplied empty password collection");
            }

            foreach (var password in passwords)
            {
                Log.Debug("PdfUtil :: trying to decrypt with Password: [" + password + "]", typeof(PdfUtil));
                var tmpdoc = Decrypt(doc, password);
                if (tmpdoc != null)
                {
                    return tmpdoc;
                }
            }

            return null;
        }


        /// <summary>
        /// Tries to decrypt a document with the given password
        /// </summary>
        /// <param name="doc">Document of type PDDocument</param>
        /// <param name="password">Password of type string</param>
        /// <returns>decrypted document (PDDocument) or null if decryption fails</returns>
        private static PDDocument Decrypt(PDDocument doc, string password)
        {
            var standardDecryptionMaterial = new StandardDecryptionMaterial(password);

            try
            {
                doc.openProtection(standardDecryptionMaterial);
                return doc;
            }
            catch (Exception ex)
            {
                Log.Debug("PdfUtil :: Decryption failed", ex);
                return null;
            }
        }
    }
}