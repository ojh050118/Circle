using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Extensions.ImageExtensions;
using osu.Framework.Utils;
using osuTK.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Circle.Game.Utils
{
    public static class ImageUtil
    {
        public static Color4 GetAverageColor(byte[] data)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(data))
            {
                const int width = 20;
                const int height = 20;

                image.Mutate(x => x.Resize(new Size(width, height)));

                var pixels = image.CreateReadOnlyPixelSpan().Span.ToArray().Select(x => new Color4(x.R, x.G, x.B, 1));

                KMeansClusterer clusterer = new KMeansClusterer();
                List<Cluster> clusters = clusterer.GetClusters(pixels.ToList());

                return clusters.First().GetCentroid();
            }
        }

        public static Task<Color4> GetAverageColorAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                return GetAverageColor(data);
            }, cancellationToken);
        }

        private class Cluster
        {
            public List<Color4> Points { get; }

            public Cluster()
            {
                Points = new List<Color4>();
            }

            public Color4 GetCentroid()
            {
                float r = 0;
                float g = 0;
                float b = 0;

                foreach (Color4 color in Points)
                {
                    r += color.R;
                    g += color.G;
                    b += color.B;
                }

                float centroidR = r / Points.Count;
                float centroidG = g / Points.Count;
                float centroidB = b / Points.Count;

                return new Color4(centroidR, centroidG, centroidB, 1);
            }
        }

        private class KMeansClusterer
        {
            public List<Cluster> GetClusters(List<Color4> points)
            {
                List<Cluster> clusters = initializeClusters(points);
                bool converged = false;

                while (!converged)
                {
                    List<Cluster> oldClusters = new List<Cluster>(clusters);

                    // 각 포인트를 가장 가까운 클러스터에 할당
                    foreach (Color4 point in points)
                    {
                        Cluster closestCluster = getClosestCluster(point, clusters);
                        closestCluster.Points.Add(point);
                    }

                    // 클러스터 중심을 재계산
                    foreach (Cluster cluster in clusters)
                    {
                        if (cluster.Points.Count > 0)
                        {
                            var centreCluster = cluster.GetCentroid();
                            cluster.Points.Clear(); // 재계산을 위해 기존 포인트 제거
                            cluster.Points.Add(centreCluster); // 클러스터 중심 포인트 추가
                        }
                    }

                    // 클러스터가 수렴했는지 확인
                    converged = clustersConverged(oldClusters, clusters);
                }

                return clusters;
            }

            private List<Cluster> initializeClusters(List<Color4> points)
            {
                List<Cluster> clusters = new List<Cluster>();

                // 초기 클러스터 중심을 무작위로 선택
                Random random = new Random();

                for (int i = 0; i < 1; i++)
                {
                    Cluster cluster = new Cluster();
                    int randomIndex = random.Next(0, points.Count);
                    cluster.Points.Add(points[randomIndex]);
                    clusters.Add(cluster);
                }

                return clusters;
            }

            private Cluster getClosestCluster(Color4 point, List<Cluster> clusters)
            {
                Cluster closestCluster = new Cluster();
                double minDistance = double.MaxValue;

                foreach (Cluster cluster in clusters)
                {
                    Color4 centroid = cluster.GetCentroid();
                    double distance = calculateDistance(point, centroid);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestCluster = cluster;
                    }
                }

                return closestCluster;
            }

            private double calculateDistance(Color4 point1, Color4 point2)
            {
                // 컬러 포인트 간의 유클리드 거리 계산
                double distance = Math.Sqrt(Math.Pow(point2.R - point1.R, 2) +
                                            Math.Pow(point2.G - point1.G, 2) +
                                            Math.Pow(point2.B - point1.B, 2));
                return distance;
            }

            private bool clustersConverged(List<Cluster> oldClusters, List<Cluster> newClusters)
            {
                // 클러스터 중심이 이전과 같은지 확인
                for (int i = 0; i < 1; i++)
                {
                    Color4 oldCentroid = oldClusters[i].GetCentroid();
                    Color4 newCentroid = newClusters[i].GetCentroid();

                    bool equalsR = Precision.AlmostEquals(oldCentroid.R, newCentroid.R);
                    bool equalsG = Precision.AlmostEquals(oldCentroid.G, newCentroid.G);
                    bool equalsB = Precision.AlmostEquals(oldCentroid.B, newCentroid.B);

                    if (!(equalsR && equalsG && equalsB))
                        return false;
                }

                return true;
            }
        }
    }
}
