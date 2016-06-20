using Foundation;
using UIKit;
using System;
using AVFoundation;

namespace TestScanQRCode
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate,IAVCaptureMetadataOutputObjectsDelegate
	{
		UINavigationController navController;
		UIWindow window;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			GlobalObject.TheAppDel = this;

			navController = new UINavigationController (new MainViewController ());

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = navController;
			window.MakeKeyAndVisible ();

			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}

		#region QRCode Scan

		bool canReceiveData = true;
		public bool CanReceiveData {
			get {
				return canReceiveData;
			}
			set {
				canReceiveData = value;
			}
		}

		[Export ("captureOutput:didOutputMetadataObjects:fromConnection:")]
		public void DidOutputMetadataObjects (AVFoundation.AVCaptureMetadataOutput captureOutput, AVFoundation.AVMetadataObject[] metadataObjects, AVFoundation.AVCaptureConnection connection)
		{
			if (null != captureOutput && metadataObjects.Length > 0 && canReceiveData) {
				canReceiveData = false;
				AVMetadataMachineReadableCodeObject metadataObj = metadataObjects [0] as AVMetadataMachineReadableCodeObject;
				NSString result = new NSString ();
				if (metadataObj.Type.Equals (AVMetadataObjectType.QRCode)) {
					result = new NSString( metadataObj.StringValue );
				}
				else {
					result = new NSString( "It's not a QRCode." );
				}
				this.PerformSelector (new ObjCRuntime.Selector ("reportScanResult:"),NSThread.MainThread, result, false);
			}
		}

		[Export("reportScanResult:")]
		void ReportScanResult(NSString result)
		{
			GlobalObject.TheQRCodeReaderViewController.ReturnResult (result.ToString());

			NSData data = NSData.FromFile(NSBundle.MainBundle.PathForResource ("beep", "wav"));
			NSError error = new NSError ();
			AVAudioPlayer beepPlayer = new AVAudioPlayer (data, "", out error);
			beepPlayer.Play ();
		}

		#endregion
	}
}


