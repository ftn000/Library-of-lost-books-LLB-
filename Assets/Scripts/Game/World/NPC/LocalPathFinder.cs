using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalPathfinder : MonoBehaviour
{
    public float cell = 0.25f;
    public int radius = 12;

    public LayerMask obstacleMask;

    class Node
    {
        public int x;
        public int y;

        public float g;
        public float h;

        public Node parent;

        public float F => g + h;
    }


    Vector2Int Vec(Vector3 p)
    {
        return new Vector2Int(
            Mathf.RoundToInt(p.x / cell),
            Mathf.RoundToInt(p.z / cell)
        );
    }

    Vector3 Vec(Vector2Int v)
    {
        return new Vector3(
            v.x * cell,
            0,
            v.y * cell
        );
    }

    bool Blocked(int x, int y)
    {
        Vector3 pos = Vec(new Vector2Int(x, y)) + Vector3.up * .1f;

        return Physics.CheckBox(
            pos,
            new Vector3(cell * .45f, .1f, cell * .45f),
            Quaternion.identity,
            obstacleMask
        );
    }

    public List<Vector3> BuildPath(Vector3 from, Vector3 to)
    {
        var start = Vec(from);
        var end = Vec(to);

        var open = new List<Node>();
        var closed = new HashSet<Vector2Int>();

        var s = new Node
        {
            x = start.x,
            y = start.y,
            g = 0,
            h = Vector2Int.Distance(start, end),
            parent = default
        };

        open.Add(s);

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.F.CompareTo(b.F));

            var cur = open[0];
            open.RemoveAt(0);

            var key = new Vector2Int(cur.x, cur.y);
            closed.Add(key);

            if (cur.x == end.x && cur.y == end.y)
            {
                return Reconstruct(cur);
            }

            foreach (var n in Neighbors(cur))
            {
                var nk = new Vector2Int(n.x, n.y);

                if (closed.Contains(nk)) continue;
                if (Blocked(n.x, n.y)) continue;

                var existing = open.Find(o => o.x == n.x && o.y == n.y);

                if (existing.x != 0 || existing.y != 0)
                {
                    if (existing.g <= n.g) continue;

                    open.Remove(existing);
                }

                open.Add(n);
            }
        }

        return null;
    }

    List<Node> Neighbors(Node n)
    {
        var list = new List<Node>();

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            var nx = n.x + dx[i];
            var ny = n.y + dy[i];

            var node = new Node
            {
                x = nx,
                y = ny,
                g = n.g + 1,
                h = 0,
                parent = n
            };

            list.Add(node);
        }

        return list;
    }

    List<Vector3> Reconstruct(Node n)
    {
        var path = new List<Vector3>();

        while (true)
        {
            path.Add(Vec(new Vector2Int(n.x, n.y)));

            if (n.parent.x == 0 && n.parent.y == 0)
                break;

            n = n.parent;
        }

        path.Reverse();
        return path;
    }
}
