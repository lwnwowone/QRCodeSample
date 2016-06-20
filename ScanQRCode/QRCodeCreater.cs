using System;
using CoreImage;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Runtime.InteropServices;

namespace TestScanQRCode
{
	public class QRCodeCreater
	{
		private static QRCodeCreater instance;

		public static QRCodeCreater Instance()
		{
			if (null == instance)
				instance = new QRCodeCreater ();
			return instance;
		}

		public enum QRCodeCorrectionLevel
		{
			L, // Level L (Low)	7% of codewords can be restored.
			M, // Level M (Medium) 15% of codewords can be restored.
			Q, // Level Q (Quartile) 25% of codewords can be restored.
			H  // Level H (High) 30% of codewords can be restored.
		}

		//old method
//		public UIImage GetQRCodeImageByString(string sourceStr,QRCodeCorrectionLevel level = QRCodeCorrectionLevel.H)
//		{
//			CIQRCodeGenerator qrGen = new CIQRCodeGenerator ();
//			qrGen.Message = NSData.FromString (sourceStr);
//			qrGen.CorrectionLevel = level.ToString();
//
//			var output = new CILanczosScaleTransform {
//				Image = qrGen.OutputImage,
//				Scale = 1
//			}.OutputImage;
//		
//			var context = CIContext.FromOptions(null);
//			UIImage img = UIImage.FromImage (context.CreateCGImage (output, output.Extent));
//			Console.WriteLine ("output.Extent.Size="+output.Extent.Size);
//			Console.WriteLine ("img.Size="+img.Size);
//			return img;
//		}

		public UIImage GetQRCodeImageByString(string sourceStr,nfloat length,QRCodeCorrectionLevel level = QRCodeCorrectionLevel.M)
		{
			NSString nsStr = new NSString (sourceStr);
			CIImage ciImage = CreateQRForString (nsStr,level);
			return CreateNonInterpolatedUIImageFormCIImage (ciImage, length);
		}

		CIImage CreateQRForString(NSString qrString, QRCodeCorrectionLevel level)
		{
			NSData strData = qrString.Encode(NSStringEncoding.UTF8);
			// 创建filter
			CIFilter qrFilter = CIFilter.FromName("CIQRCodeGenerator");
			// 设置内容和纠错级别
			qrFilter.SetValueForKey(strData,new NSString("inputMessage"));
			qrFilter.SetValueForKey (new NSString (level.ToString ()), new NSString("inputCorrectionLevel"));
			// 返回CIImage
			return qrFilter.OutputImage;
		}

		UIImage CreateNonInterpolatedUIImageFormCIImage(CIImage image ,nfloat size)
		{
			CGRect extent = image.Extent;
			float scale = (float)Math.Min(size/extent.Width,size/extent.Height);
			// 创建bitmap;
			int width = (int)(extent.Width * scale);
			int height = (int)(extent.Height * scale);
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceGray();
//			var rawData = Marshal.AllocHGlobal(height * width * 4);
			CGBitmapContext bitmapRef = new CGBitmapContext (null, width, height, 8, 0, colorSpace, CGImageAlphaInfo.None);
			CIContext context = CIContext.FromOptions (null);
			CGImage bitmapImage = context.CreateCGImage (image, extent);
			bitmapRef.InterpolationQuality = CGInterpolationQuality.None;
			bitmapRef.ScaleCTM (scale, scale);
			bitmapRef.DrawImage (extent, bitmapImage);
			// 保存bitmap到图片
			CGImage scaledImage = bitmapRef.ToImage();
			return new UIImage (scaledImage);
		}

//		void CalculateLuminance(UIImage d)
//		{
//			var imageRef = d.CGImage;
//			var width = (int)imageRef.Width;
//			var height = (int)imageRef.Height;
//			var colorSpace = CGColorSpace.CreateDeviceRGB();
//
//			var rawData = Marshal.AllocHGlobal(height * width * 4);
//
//			try
//			{
//				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little; 
//				var context = new CGBitmapContext(rawData, width, height, 8, 4 * width,
//					colorSpace, (CGImageAlphaInfo)flags);
//
//				context.DrawImage(new CGRect(0.0f, 0.0f, (float)width, (float)height), imageRef);
//				var pixelData = new byte[height * width * 4];
//				Marshal.Copy(rawData, pixelData, 0, pixelData.Length);
//
//				CalculateLuminance(pixelData, BitmapFormat.BGRA32);
//			}
//			finally
//			{
//				Marshal.FreeHGlobal(rawData);
//			}
//		}
	}
}

