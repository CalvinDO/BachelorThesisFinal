Shader "CW/HandleShader"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 3D) = "white"
	}

	SubShader
	{
		Tags { "Queue" = "Overlay+1" }
		ZTest Always

		Pass
		{
			SetTexture[_MainTex]
			{
				constantColor[_Color]
			}

		}
	}
}