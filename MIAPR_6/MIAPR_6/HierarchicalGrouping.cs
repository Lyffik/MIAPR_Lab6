using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace MIAPR_6
{
    public class HierarchicalGrouping
    {
        private readonly List<Color> colors = new List<Color>
        {
            Color.Red,
            Color.Blue,
            Color.DodgerBlue,
            Color.DarkMagenta,
            Color.BlueViolet,
            Color.DeepPink,
            Color.Firebrick,
            Color.ForestGreen,
            Color.MidnightBlue,
            Color.Green,
            Color.Aqua,
            Color.DarkOrchid,
            Color.RoyalBlue,
            Color.Bisque,
            Color.Beige,
            Color.Salmon,
            Color.Sienna,
            Color.PowderBlue,
            Color.Plum,
            Color.LightSalmon,
            Color.Khaki,
            Color.Olive,
            Color.YellowGreen,
            Color.Violet,
            Color.Gold,
            Color.Yellow,
            Color.LightCyan,
            Color.LightBlue,
            Color.Turquoise
        };

        private readonly List<Group> groups = new List<Group>();
        private int DX = 10;
        private int nextColor;

        private int numberOfChar;

        public HierarchicalGrouping(double[,] distances, int size)
        {
            for (int i = 0; i < size; i++)
            {
                groups.Add(new Group());
                groups[i].Name = "X" + (i + 1);
            }
            for (int i = 0; i < groups.Count; i++)
            {
                for (int j = 0; j < groups.Count; j++)
                {
                    if (i != j)
                    {
                        groups[i].Distances.Add(new Distances(distances[i, j], groups[j]));
                    }
                }
            }
        }

        private char NextChar()
        {
            const string alphabet = "ABCDEFGHIKLMNOPQRSTVXYZ";
            numberOfChar++;
            return alphabet[numberOfChar - 1];
        }

        private void AddGroups(List<Group> addedGroups, double minDistance)
        {
            var newGroup = new Group();
            newGroup.Name = NextChar().ToString();
            foreach (Group group in groups)
            {
                if (!addedGroups.Contains(group))
                {
                    double minDist = group.GetDistance(addedGroups[0]);
                    foreach (Group currGroup in addedGroups)
                    {
                        if (group.GetDistance(currGroup) < minDist)
                        {
                            minDist = group.GetDistance(currGroup);
                        }
                    }
                    group.DeleteDistances(addedGroups);
                    group.Distances.Add(new Distances(minDist, newGroup));
                    newGroup.Distances.Add(new Distances(minDist, group));
                }
            }
            foreach (Group addedGroup in addedGroups)
            {
                if (addedGroup.X == 0)
                {
                    addedGroup.X = DX;
                    DX += 10;
                }
            }
            newGroup.SubGroups = addedGroups;
            var subGroupsPoints = new List<DPoint>();
            foreach (Group addedGroup in addedGroups)
            {
                subGroupsPoints.Add(new DPoint(addedGroup.X, addedGroup.Y));
                groups.Remove(addedGroup);
            }
            double xCoord = 0;
            foreach (DPoint point in subGroupsPoints)
            {
                xCoord += point.X;
            }
            newGroup.X = xCoord/subGroupsPoints.Count;
            newGroup.Y = minDistance;
            groups.Add(newGroup);
        }

        public void FindGroups()
        {
            bool result = false;
            do
            {
                result = false;
                var groupWithMinDist = new List<Group>();
                double minDistance = 100;
                foreach (Group group in groups)
                {
                    foreach (Distances distance in group.Distances)
                    {
                        if (distance.Distance <= minDistance)
                        {
                            if (distance.Distance < minDistance)
                            {
                                minDistance = distance.Distance;
                                result = true;
                                groupWithMinDist.Clear();
                            }
                            groupWithMinDist.Add(group);
                        }
                    }
                }
                if (result && groupWithMinDist.Count > 1)
                {
                    AddGroups(groupWithMinDist, minDistance);
                }
            } while (result);
        }

        private void SetUpChart(Chart chart,int i)
        {
            chart.Series.Clear();
            chart.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.Lines;
            chart.ChartAreas[0].AxisX.Crossing = 0;
            chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisX.LineWidth = 2;
            chart.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.Lines;
            chart.ChartAreas[0].AxisY.Crossing = 0;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = DX;
            chart.ChartAreas[0].AxisY.Maximum = groups[0].Y + 0.01;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Title = "R";
            if (i==0)
            {
                chart.ChartAreas[0].AxisY.Interval = 1;
            }
            else
            {
                chart.ChartAreas[0].AxisY.Interval = groups[0].Y / 10;
            }
            chart.ChartAreas[0].AxisY.LineWidth = 2;
        }

        private void DrawSubGroups(Group group, Chart chart)
        {
            bool res = true;
            foreach (Series currSeries in chart.Series)
            {
                if (currSeries.Name == group.Name)
                {
                    res = false;
                }
            }
            if (res)
            {
                var pointsSeries = new Series {ChartType = SeriesChartType.Point};
                pointsSeries.Name = group.Name;
                pointsSeries.MarkerSize = 30;
                pointsSeries.MarkerColor = colors[nextColor];
                nextColor++;
                pointsSeries.Points.AddXY(group.X, group.Y);
                chart.Series.Add(pointsSeries);
                foreach (Group subGroup in group.SubGroups)
                {
                    var lineSeries = new Series {ChartType = SeriesChartType.Line};
                    lineSeries.BorderWidth = 4;
                    lineSeries.Name = group.Name + "<->" + subGroup.Name;
                    lineSeries.Color = pointsSeries.MarkerColor;
                    lineSeries.Points.AddXY(subGroup.X, subGroup.Y);
                    lineSeries.Points.AddXY(subGroup.X, group.Y);
                    lineSeries.Points.AddXY(group.X, group.Y);
                    res = true;
                    foreach (Series currSeries in chart.Series)
                    {
                        if (currSeries.Name == lineSeries.Name)
                        {
                            res = false;
                        }
                    }
                    if (res)
                    {
                        chart.Series.Add(lineSeries);
                    }
                    DrawSubGroups(subGroup, chart);
                }
            }
        }

        public void Draw(Chart chart,int i)
        {
            SetUpChart(chart,i);
            foreach (Group subGroup in groups)
            {
                DrawSubGroups(subGroup, chart);
            }
        }

        private struct DPoint
        {
            public double X;
            public double Y;

            public DPoint(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private struct Distances
        {
            public readonly double Distance;
            public readonly Group Group;

            public Distances(double dist, Group sub)
            {
                Distance = dist;
                Group = sub;
            }
        }

        private class Group
        {
            public List<Distances> Distances = new List<Distances>();
            public string Name;
            public List<Group> SubGroups = new List<Group>();
            public double X;
            public double Y;

            public double GetDistance(Group group)
            {
                foreach (Distances distance in Distances)
                {
                    if (distance.Group.Equals(group))
                    {
                        return distance.Distance;
                    }
                }
                return -1;
            }

            public void DeleteDistances(List<Group> deleteList)
            {
                foreach (Group deleteGroup in deleteList)
                {
                    var deleteDistances = new List<Distances>();
                    foreach (Distances distance in Distances)
                    {
                        if (distance.Group.Equals(deleteGroup))
                        {
                            deleteDistances.Add(distance);
                        }
                    }
                    foreach (Distances deleteDistance in deleteDistances)
                    {
                        Distances.Remove(deleteDistance);
                    }
                }
            }
        }
    }
}