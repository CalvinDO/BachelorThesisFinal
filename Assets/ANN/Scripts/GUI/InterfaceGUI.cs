using UnityEngine;

/// <summary>
/// InterfaceGUI for ANN interfaces.
/// </summary>
public static class InterfaceGUI
{
    /// <summary>
    /// Button.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name for display.</param>
    /// <returns>Returns bool value.</returns>
    public static bool Button(int Column, int Line, string Name)
    {
        bool Temp = Button(Column, Line, Name, Name, false);
        return Temp;
    }

    /// <summary>
    /// Button.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name for display.</param>
    /// <param name="Parameter">Bool value.</param>
    /// <returns>Returns bool value.</returns>
    public static bool Button(int Column, int Line, string Name, bool Parameter)
    {
        bool Temp = Button(Column, Line, Name, Name, Parameter);
        return Temp;
    }

    /// <summary>
    /// Button. With activation bool value.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="NameFalse">Name for false value.</param>
    /// <param name="NameTrue">Name for true value.</param>
    /// <param name="Parameter">Bool value.</param>
    /// <param name="Activate">Reflect of additional bool value. Always true on press.</param>
    /// <returns>Returns bool value.</returns>
    public static bool Button(int Column, int Line, string NameFalse, string NameTrue, bool Parameter, ref bool Activate)
    {
        bool Temp = Button(Column, Line, NameFalse, NameTrue, Parameter);
        if (Temp != Parameter)
            Activate = true;
        return Temp;
    }

    /// <summary>
    /// Button.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="NameFalse">Name for false value.</param>
    /// <param name="NameTrue">Name for true value.</param>
    /// <param name="Parameter">Bool value.</param>
    /// <returns>Returns bool value.</returns>
    public static bool Button(int Column, int Line, string NameFalse, string NameTrue, bool Parameter)
    {
        string TempString = NameFalse;
        if (Parameter)
            TempString = NameTrue;
        if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), TempString))
        {
            if (Parameter)
                Parameter = false;
            else
                Parameter = true;
        }
        return Parameter;
    }

    /// <summary>
    /// Float horizontal slider. With activation bool value.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of horizontal slider.</param>
    /// <param name="Parameter">Float value.</param>
    /// <param name="Min">Minimum value.</param>
    /// <param name="Max">Maximum value.</param>
    /// <param name="Activate">Reflect of additional bool value. Always true when changing рarameter.</param>
    /// <returns>Returns float value.</returns>
    public static float HorizontalSlider(int Column, int Line, string Name, float Parameter, float Min, float Max, ref bool Activate)
    {
        float Temp = HorizontalSlider(Column, Line, Name, Parameter, Min, Max);

        if (Temp != Parameter)
            Activate = true;
        return Temp;
    }

    /// <summary>
    /// Float horizontal slider.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of horizontal slider.</param>
    /// <param name="Parameter">Float value.</param>
    /// <param name="Min">Minimum value.</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns float value.</returns>
    public static float HorizontalSlider(int Column, int Line, string Name, float Parameter, float Min, float Max)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");

        Parameter = GUI.HorizontalSlider(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30 + 18, 180, 12), Parameter, Min, Max);
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 22), Name + ": " + Parameter.ToString("f2"));
        return Parameter;
    }

    /// <summary>
    /// Integer horizontal slider.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of horizontal slider.</param>
    /// <param name="Parameter">Integer value.</param>
    /// <param name="Min">Minimum value.</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns float value.</returns>
    public static int HorizontalSlider(int Column, int Line, string Name, float Parameter, int Min, int Max)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");

        Parameter = GUI.HorizontalSlider(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30 + 18, 180, 12), Parameter, Min, Max);
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 22), Name + ": " + Parameter.ToString("f0"));
        return (int)Parameter;
    }

    /// <summary>
    /// Float horizontal slider with showing correction.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of horizontal slider.</param>
    /// <param name="Parameter">Float value.</param>
    /// <param name="ParameterShow">Float value. Correction.</param>
    /// <param name="Min">Minimum value.</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns float value.</returns>
    public static float HorizontalSlider(int Column, int Line, string Name, float Parameter, float ParameterShow, float Min, float Max)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");

        Parameter = GUI.HorizontalSlider(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30 + 18, 180, 12), Parameter, Min, Max);
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 22), Name + ": " + ParameterShow.ToString("f2"));
        return Parameter;
    }

    /// <summary>
    /// Int arrows. Maximum value is infinity.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of int value</param>
    /// <param name="Parameter">Int value.</param>
    /// <param name="Min">Minimum value</param>
    /// <returns>Returns int value.</returns>
    public static int IntArrows(int Column, int Line, string Name, int Parameter, int Min)
    {
        int Temp = IntArrows(Column, Line, Name, false, Parameter, Min, true, 0);
        return Temp;
    }

    /// <summary>
    /// Int arrows.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of int value</param>
    /// <param name="Parameter">Int value.</param>
    /// <param name="Min">Minimum value</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns int value.</returns>
    public static int IntArrows(int Column, int Line, string Name, int Parameter, int Min, int Max)
    {
        int Temp = IntArrows(Column, Line, Name, false, Parameter, Min, false, Max);
        return Temp;
    }

    /// <summary>
    /// Int arrows. With plus one to displayed value if "OnePlus" is true.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of int value</param>
    /// <param name="OnePlus">Int value for display. Correction +1.</param>
    /// <param name="Parameter">Int value.</param>
    /// <param name="Min">Minimum value</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns int value.</returns>
    public static int IntArrows(int Column, int Line, string Name, bool OnePlus, int Parameter, int Min, int Max)
    {
        int Temp = IntArrows(Column, Line, Name, OnePlus, Parameter, Min, false, Max);
        return Temp;
    }

    /// <summary>
    /// Int arrows. With activation bool value. Maximum value is infinity.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of int value</param>
    /// <param name="Parameter">Int value.</param>
    /// <param name="Min">Minimum value</param>
    /// <param name="Activate">Reflect of additional bool value. Always true when changing рarameter.</param>
    /// <returns>Returns int value.</returns>
    public static int IntArrows(int Column, int Line, string Name, int Parameter, int Min, ref bool Activate)
    {
        int Temp = IntArrows(Column, Line, Name, false, Parameter, Min, true, 0);
        if (Temp != Parameter)
            Activate = true;
        return Temp;
    }

    /// <summary>
    /// Int arrows.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of int value</param>
    /// <param name="OnePlus">Int value for display. Correction +1.</param>
    /// <param name="Parameter">Int value.</param>
    /// <param name="Min">Minimum value</param>
    /// <param name="Infinity">Maximum value is infinity?</param>
    /// <param name="Max">Maximum value.</param>
    /// <returns>Returns int value.</returns>
    public static int IntArrows(int Column, int Line, string Name, bool OnePlus, int Parameter, int Min, bool Infinity, int Max)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");

        GUI.Label(new Rect(10 * Column + (Column - 1) * 180 + 31, 25 + (Line - 1) * 30, 100, 22), Name + ": ");
        if (Min != Max || Infinity)
        {
            if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 13, 30), "|"))
            {
                Parameter = Min;
            }
            if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180 + 13, 25 + (Line - 1) * 30, 18, 30), "<"))
            {
                Parameter--;
            }
            if (Infinity)
            {
                if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180 + 162, 25 + (Line - 1) * 30, 18, 30), ">"))
                {
                    Parameter++;
                }
            }
            else
            {
                if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180 + 167, 25 + (Line - 1) * 30, 13, 30), "|"))
                {
                    Parameter = Max;
                }
                if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180 + 149, 25 + (Line - 1) * 30, 18, 30), ">"))
                {
                    Parameter++;
                }
                Parameter = (int)GUI.HorizontalSlider(new Rect(10 * Column + (Column - 1) * 180 + 31, 25 + (Line - 1) * 30 + 18, 118, 12), Parameter, Min, Max);
            }
        }
        if (Parameter < Min)
            Parameter = Min;
        if (Parameter > Max && !Infinity)
            Parameter = Max;
        Name = Parameter.ToString();
        if (OnePlus)
            Name = (Parameter + 1).ToString();
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180 + 131, 25 + (Line - 1) * 30, 30, 30), Name);
        return Parameter;
    }

    /// <summary>
    /// Shows float value.
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of value</param>
    /// <param name="Parameter">Float value</param>
    public static void Info(int Column, int Line, string Name, float Parameter)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 22), " " + Name + ": " + Parameter);
    }

    /// <summary>
    /// Shows float value with "f2".
    /// </summary>
    /// <param name="Column">Column of button. Start from 1.</param>
    /// <param name="Line">Line of button. Start from 1.</param>
    /// <param name="Name">Name of value</param>
    /// <param name="Parameter">Float value</param>
    public static void InfoF2(int Column, int Line, string Name, float Parameter)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 30), "");
        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 25 + (Line - 1) * 30, 180, 22), " " + Name + ": " + Parameter.ToString("f2"));
    }
}
