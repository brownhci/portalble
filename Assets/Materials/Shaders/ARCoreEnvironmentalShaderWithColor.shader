Shader "ARCore/ColorDiffuseWithLightEstimation"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd finalcolor:lightEstimation

		fixed4 _Color;
		fixed3 _GlobalColorCorrection;

		struct Input
		{
			float2 uv_MainTex;
		};

		void lightEstimation(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			color.rgb *= _GlobalColorCorrection;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

		Fallback "Mobile/VertexLit"
}