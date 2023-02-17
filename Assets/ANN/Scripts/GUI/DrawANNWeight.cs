using System;
using UnityEngine;

/// <summary>
/// Draw ANN weight.
/// </summary>
public static class DrawANNWeight {
    private static Texture2D texture = new Texture2D(1, 1);
    private static Rect lineRect = new Rect(0, 0, 1, 1);

    /// <summary>
    /// ANN’s weight visualization.
    /// </summary>
    /// <param name="pointA">Start neuron position on visualization window.</param>
    /// <param name="pointB">End neuron position on visualization window.</param>
    /// <param name="width">Width of line.</param>
    /// <param name="sum">Weight * neuron.</param>
    /// <param name="fade">If it "true" then will fade lines. If it "false" then will not fade lines.</param>
    public static void Line(Vector2 pointA, Vector2 pointB, float width, float sum, bool fade) {
        Color color = Color.white;
        if (width > 0)
            color = new Color(0, width, 0, 0.1F);
        else {
            color = new Color(-width, 0, 0, 0.1F);
            width = -width;
        }
        Line(pointA, pointB, color, width, sum, fade);
    }

    /// <summary>
    /// ANN’s weight visualization.
    /// </summary>
    /// <param name="pointA">Start neuron position on visualization window.</param>
    /// <param name="pointB">End neuron position on visualization window.</param>
    /// <param name="width">Width of line.</param>
    /// <param name="fade">If it true then will fade lines. If it false then will not fade lines.</param>
    public static void Line(Vector2 pointA, Vector2 pointB, float width, bool fade) {
        Line(pointA, pointB, Color.blue, width, 1, fade);
    }

    private static void Line(Vector2 pointA, Vector2 pointB, Color color, float width, float sum, bool fade) {
        float Dx = pointB.x - pointA.x;
        float Dy = pointB.y - pointA.y;

        float Length = Mathf.Sqrt(Dx * Dx + Dy * Dy);

        if (Length < 0.001f)
            return;

        float wDx = width * Dy / Length;
        float wDy = width * Dx / Length;

        float sumX = 0;
        float sumY = 0;

        if (fade)
            color.a = sum;
        else {
            sum = 1 - sum;
            sumX = -wDx * sum / 2F;
            sumY = wDy * sum / 2F;
            color.a = 1;
        }

        Matrix4x4 Matrix = Matrix4x4.identity;
        Matrix.m00 = Dx + sumX;
        Matrix.m01 = -wDx;
        Matrix.m03 = pointA.x + 0.5F * wDx;
        Matrix.m10 = Dy + sumY;
        Matrix.m11 = wDy;
        Matrix.m13 = pointA.y - 0.5F * wDy;
        if (!fade) {
            Matrix.m20 = sum;
            Matrix.m21 = 1F;
        }

        GL.PushMatrix();
        GL.MultMatrix(Matrix);

        if (texture) {


            try {

                Graphics.DrawTexture(lineRect, texture, lineRect, 0, 0, 0, 0, color, null);
            }
            catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
        GL.PopMatrix();
    }
}