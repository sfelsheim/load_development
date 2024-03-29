﻿using Foundation;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.XForms.iOS.EffectsView;
using UIKit;

namespace LoadDevelopmentUI.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            new Syncfusion.XForms.iOS.ComboBox.SfComboBoxRenderer();
            global::Xamarin.Forms.Forms.Init();
            Syncfusion.XForms.iOS.PopupLayout.SfPopupLayoutRenderer.Init();
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            Forms9Patch.iOS.Settings.Initialize(this);
            FileAccessHelper.GetLocalFilePath("master.db");
            Syncfusion.SfImageEditor.XForms.iOS.SfImageEditorRenderer.Init();

            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();

            LoadApplication(new App());
            Syncfusion.XForms.iOS.Buttons.SfRadioButtonRenderer.Init();

            return base.FinishedLaunching(app, options);
        }
    }
}
