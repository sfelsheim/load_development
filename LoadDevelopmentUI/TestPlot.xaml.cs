using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace LoadDevelopmentUI
{
    public partial class TestPlot : ContentPage
    {
        private ModelView.TestLoadModelView modelView;
        private ModelView.ChartModelView chartModelView;

        public TestPlot(ModelView.TestLoadModelView mv)
        {
            modelView = mv;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            chartModelView = new ModelView.ChartModelView(modelView);
            this.BindingContext = chartModelView;
        }

        void AllShots_CheckBox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            chartModelView.DrawAllShots = e.Value;
            // does not seem to want to update :(
            //chartModelView.UpdateGraph();
            chartModelView.CreatePlotModel();
        }

        void Gridlines_CheckBox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            chartModelView.DrawGridlines = e.Value;
            //chartModelView.UpdateGraph();
            chartModelView.CreatePlotModel();
        }
    }
}
