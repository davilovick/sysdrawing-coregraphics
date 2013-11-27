using System;
using System.Drawing.Text;

#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
#endif

namespace System.Drawing
{
	public sealed partial class FontFamily
	{
		const string MONO_SPACE = "Courier";
		const string SANS_SERIF = "Helvetica";
		const string SERIF = "Times";

		CTFontDescriptor nativeFontDescriptor;

		internal CTFontDescriptor NativeDescriptor
		{
			get { return nativeFontDescriptor; }
		}

		private void CreateNativeFontFamily(string name, bool createDefaultIfNotExists)
		{
			CreateNativeFontFamily (name, null, createDefaultIfNotExists);
		}

		private void CreateNativeFontFamily(string name, FontCollection fontCollection, bool createDefaultIfNotExists)
		{
			if (fontCollection != null) 
			{
				if (fontCollection.nativeFontDescriptors.ContainsKey (name))
					nativeFontDescriptor = fontCollection.nativeFontDescriptors [name];

				if (nativeFontDescriptor == null && createDefaultIfNotExists) 
				{
					nativeFontDescriptor = new CTFontDescriptor (SANS_SERIF, 0);
				}
			} 
			else 
			{
				nativeFontDescriptor = new CTFontDescriptor (name, 0);

				if (nativeFontDescriptor == null && createDefaultIfNotExists) 
				{
					nativeFontDescriptor = new CTFontDescriptor (SANS_SERIF, 0);
				}
			}

			if (nativeFontDescriptor == null)
				throw new ArgumentException ("name specifies a font that is not installed on the computer running the application.");
			else 
			{
				var attrs = nativeFontDescriptor.GetAttributes ();
				familyName = attrs.FamilyName;
				// if the font description attributes do not contain a value for FamilyName then we
				// need to try and create the font to get the family name from the actual font.
				if (string.IsNullOrEmpty (familyName)) 
				{
					var font = new CTFont (nativeFontDescriptor, 0);
					familyName = font.FamilyName;
				}
			}

		}

		private bool nativeStyleAvailable(FontStyle style)
		{

			// we are going to actually have to create a font object here
			// will not create an actual variable for this yet.  We may
			// want to do this in the future so that we do not have to keep 
			// creating it over and over again.
			var font = new CTFont (nativeFontDescriptor,0);

			switch (style) 
			{
			case FontStyle.Bold:
				var tMaskBold = CTFontSymbolicTraits.None;
				tMaskBold |= CTFontSymbolicTraits.Bold;
				var bFont = font.WithSymbolicTraits (0, tMaskBold, tMaskBold);
				if (bFont == null)
					return false;
				var bold = (bFont.SymbolicTraits & CTFontSymbolicTraits.Bold) == CTFontSymbolicTraits.Bold;
				return bold;

			case FontStyle.Italic:
				//return (font.SymbolicTraits & CTFontSymbolicTraits.Italic) == CTFontSymbolicTraits.Italic; 
				var tMaskItalic = CTFontSymbolicTraits.None;
				tMaskItalic |= CTFontSymbolicTraits.Italic;
				var iFont = font.WithSymbolicTraits (0, tMaskItalic, tMaskItalic);
				if (iFont == null)
					return false;
				var italic = (iFont.SymbolicTraits & CTFontSymbolicTraits.Italic) == CTFontSymbolicTraits.Italic;
				return italic;

			case FontStyle.Regular:

				// Verify if this is correct somehow - we may need to add Bold here as well not sure
				if ((font.SymbolicTraits & CTFontSymbolicTraits.Condensed) == CTFontSymbolicTraits.Condensed
					||  (font.SymbolicTraits & CTFontSymbolicTraits.Expanded) == CTFontSymbolicTraits.Expanded)
					return false;
				else
					return true;
			case FontStyle.Underline:
				return font.UnderlineThickness > 0;
			case FontStyle.Strikeout:
				// not implemented yet
				return false;

			}
			return false;
		}

	}
}
