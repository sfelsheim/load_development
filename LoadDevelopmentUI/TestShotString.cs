using System;

using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public class TestShotString : ContentPage
    {
        public TestShotString()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

