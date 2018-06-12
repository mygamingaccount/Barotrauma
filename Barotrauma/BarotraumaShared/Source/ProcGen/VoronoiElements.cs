/*
 * Created by SharpDevelop.
 * User: Burhan
 * Date: 17/06/2014
 * Time: 09:29 م
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

/*
      Copyright 2011 James Humphreys. All rights reserved.
    
    Redistribution and use in source and binary forms, with or without modification, are
    permitted provided that the following conditions are met:
    
       1. Redistributions of source code must retain the above copyright notice, this list of
          conditions and the following disclaimer.
    
       2. Redistributions in binary form must reproduce the above copyright notice, this list
          of conditions and the following disclaimer in the documentation and/or other materials
          provided with the distribution.
    
    THIS SOFTWARE IS PROVIDED BY James Humphreys ``AS IS\" AND ANY EXPRESS OR IMPLIED
    WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
    FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
    ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    
    The views and conclusions contained in the software and documentation are those of the
    authors and should not be interpreted as representing official policies, either expressed
    or implied, of James Humphreys.
 */

/*
 * C# Version by Burhan Joukhadar
 * 
 * Permission to use, copy, modify, and distribute this software for any
 * purpose without fee is hereby granted, provided that this entire notice
 * is included in all copies of any software which is or includes a copy
 * or modification of this software and in all copies of the supporting
 * documentation for such software.
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY.  IN PARTICULAR, NEITHER THE AUTHORS NOR AT&T MAKE ANY
 * REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY
 * OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 */


using Barotrauma;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Voronoi2
{
    public class Point
    {
        public double x, y;
        
        public void setPoint ( double x, double y )
        {
            this.x = x;
            this.y = y;
        }
    }
    
    // use for sites and vertecies
    public class Site
    {
        public Point coord;
        public int sitenbr;

        public void SetPoint(Vector2 point)
        {
            coord.setPoint(point.X, point.Y);
        }
        
        public Site ()
        {
            coord = new Point();
        }
    }
    
    public class Edge
    {
        public double a = 0, b = 0, c = 0;
        public Site[] ep;
        public Site[] reg;
        public int edgenbr;
        
        public Edge ()
        {
            ep = new Site[2];
            reg = new Site[2];
        }
    }
    
    
    public class Halfedge
    {
        public Halfedge ELleft, ELright;
        public Edge ELedge;
        public bool deleted;
        public int ELpm;
        public Site vertex;
        public double ystar;
        public Halfedge PQnext;
        
        public Halfedge ()
        {
            PQnext = null;
        }
    }

    public enum CellType
    {
        Solid, Empty, Edge, Path, Removed
    }

    public class VoronoiCell
    {
        public List<GraphEdge> edges;
        public Site site;

        public List<Vector2> bodyVertices;

        public Body body;

        public CellType CellType;

        public Vector2 Translation;

        public Vector2 Center
        {
            get { return new Vector2((float)site.coord.x, (float)site.coord.y)+Translation; }
        }

        public VoronoiCell(Vector2[] vertices)
        {
            edges = new List<GraphEdge>();
            bodyVertices = new List<Vector2>();

            Vector2 midPoint = Vector2.Zero;
            foreach (Vector2 vertex in vertices)
            {
                midPoint += vertex;
            }
            midPoint /= vertices.Length;


            for (int i = 1; i < vertices.Length; i++ )
            {
                GraphEdge ge = new GraphEdge(vertices[i-1], vertices[i]);

                System.Diagnostics.Debug.Assert(ge.Point1 != ge.Point2);

                edges.Add(ge);
            }

            GraphEdge lastEdge = new GraphEdge(vertices[0], vertices[vertices.Length-1]);

            edges.Add(lastEdge);

            site = new Site();
            site.SetPoint(midPoint);
        }

        public VoronoiCell(Site site)
        {
            edges = new List<GraphEdge>();
            bodyVertices = new List<Vector2>();
            //bodies = new List<Body>();
            this.site = site;
        }

        public bool IsPointInside(Vector2 point)
        {
            foreach (GraphEdge edge in edges)
            {
                if (MathUtils.LinesIntersect(point, Center, edge.Point1, edge.Point2)) return false;
            }

            return true;
        }
    }
    
    public class GraphEdge
    {
        public Vector2 Point1, Point2;
        public Site Site1, Site2;
        public VoronoiCell Cell1, Cell2;

        public bool IsSolid;

        public bool OutsideLevel;

        public Vector2 Center
        {
            get { return (Point1 + Point2) / 2.0f; }
        }

        public GraphEdge(Vector2 point1, Vector2 point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public VoronoiCell AdjacentCell(VoronoiCell cell)
        {
            if (Cell1 == cell)
            {
                return Cell2;
            }
            else if (Cell2 == cell)
            {
                return Cell1;
            }

            return null;
        }

        /// <summary>
        /// Returns the normal of the edge that points outwards from the specified cell
        /// </summary>
        public Vector2 GetNormal(VoronoiCell cell)
        {
            Vector2 dir = Vector2.Normalize(Point1 - Point2);
            Vector2 normal = new Vector2(dir.Y, -dir.X);
            if (cell != null && Vector2.Dot(normal, Vector2.Normalize(Center - cell.Center)) < 0)
            {
                normal = -normal;
            }
            return normal;
        }
    }
    
    // للترتيب
    public class SiteSorterYX : IComparer<Site>
    {
        public int Compare ( Site p1, Site p2 )
        {
            Point s1 = p1.coord;
            Point s2 = p2.coord;
            if ( s1.y < s2.y )    return -1;
            if ( s1.y > s2.y ) return 1;
            if ( s1.x < s2.x ) return -1;
            if ( s1.x > s2.x ) return 1;
            return 0;
        }
    }
}
