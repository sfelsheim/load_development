using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess;
using DataAccess.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LoadDevelopmentUI.ModelView
{
    public class ChartModelView : INotifyPropertyChanged
    {
        private TestLoadModelView testModelView;
        private LoadDatabase database;
        private PlotModel plotModel = null;


        public ChartModelView(TestLoadModelView testModelView)
        {
            this.testModelView = testModelView;
            this.database = testModelView.Database;
            CreatePlotModel();
        }

        public void CreatePlotModel()
        { 
            Model = createTestLoadModel(testModelView.ShotStrings);
	    }

        public bool DrawGridlines { get; set; } = false;

        public void UpdateGraph()
        {
            if (plotModel != null)
            {
                updateGridLines();
                updateShowShots();
                Model = plotModel;
            }
        }

        private void updateShowShots()
        {
            foreach(var series in plotModel.Series)
            { 
                if (series.Title.Equals("Velocity"))
                {
                    series.IsVisible = DrawAllShots;
                    return;
		        }
	        }
        }

        private void updateGridLines()
        {
            foreach(var axis in plotModel.Axes)
            { 
                if (axis.IsXyAxis())
                {
                    axis.MajorGridlineStyle = DrawGridlines ? LineStyle.Dash : LineStyle.None;
                    axis.MinorGridlineStyle = DrawGridlines ? LineStyle.Dash : LineStyle.None;
		        }
	        }
        }

        public bool DrawAllShots { get; set; } = false;

        private PlotModel createTestLoadModel(List<LoadString> shotStrings)
        {
            var model = new PlotModel
            {
                Title = "Velocity by Powder Charge",
                PlotType = PlotType.XY
            };

            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Powder Charge",
                MajorGridlineStyle = DrawGridlines ? LineStyle.Dash : LineStyle.None,
                MinorGridlineStyle = DrawGridlines ? LineStyle.Dash : LineStyle.None
            });

            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Velocity"
            });

            var lineSeries = new LineSeries()
            {
                Title = "Avg Velocity",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                //LabelFormatString = "{1}",
                LabelFormatString = "{Y}",
                MarkerStroke = OxyColors.DarkBlue,
                MarkerFill = OxyColors.SkyBlue,
                MarkerStrokeThickness = 1.5,
                Color = OxyColors.SkyBlue

            };

            var nodeSeries = new LineSeries()
            {
                Title = "Velocity Node",
                MarkerType = MarkerType.Diamond,
                MarkerSize = 4,
                MarkerStroke = OxyColors.DarkGreen,
                MarkerFill = OxyColors.Green,
                LabelFormatString = "{X:F1}",
                MarkerStrokeThickness = 1.5,
                LineStyle = LineStyle.None,
            };

            var shotSeries = new LineSeries()
            {
                Title = "Velocity",
                MarkerType = MarkerType.Square,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                MarkerFill = OxyColors.Black,
                MarkerStrokeThickness = 1.5,
                LineStyle = LineStyle.None,
                IsVisible = DrawAllShots
            };

            var avgPoints = new List<DataPoint>();

            foreach (LoadString ls in shotStrings )
            {
                if (ls.AvgVelocity > 0)
			        avgPoints.Add(new DataPoint(ls.PowderCharge, ls.AvgVelocity));
	        }

            lineSeries.ItemsSource = avgPoints;
            nodeSeries.ItemsSource = calculateVelocityNodes(shotStrings);
            shotSeries.ItemsSource = calculateShotPoints(shotStrings);

            model.Series.Add(lineSeries);
            model.Series.Add(nodeSeries);
            model.Series.Add(shotSeries);

            return model;
        }

        private List<DataPoint> calculateShotPoints(List<LoadString> shotStrings)
        {
            var shotPoints = new List<DataPoint>();

            foreach (LoadString ls in shotStrings )
            {
                List<LoadStringShot> shots = database.GetLoadStringShots(ls.LoadID, ls.LoadStringID);
                foreach(LoadStringShot lss in shots)
                {
                    // ignore error or not set values
                    if (lss.Velocity <= 0)
                        continue;

                    shotPoints.Add(new DataPoint(ls.PowderCharge, lss.Velocity));
		        }
	        }

            return shotPoints;
        }

        private List<DataPoint> calculateVelocityNodes(List<LoadString> shotStrings)
        {
            return Helper.VelocityNodeCalc.CalculateVelocityNodes(shotStrings);
        }

        public PlotModel Model 
	    {
            get { return plotModel; }
	        set
            { 
                if (value != plotModel)
                {
                    plotModel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
                }
	        } 
	    }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
