Shader "Render Depth" {
	Properties{
        point_size("Point Size", Float) = 5.0
	}
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
	        {
	           float4 vertex : POSITION;
	        };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 depth : TEXCOORD0;
                float size : PSIZE;
            };

            float point_size;

            v2f vert (appdata_base v) {
                v2f o;

                // Vertex position
                o.pos = UnityObjectToClipPos(v.vertex);

                // Set depth, pass to fragment shader
                UNITY_TRANSFER_DEPTH(o.depth);

                // Set point size (geometry shader)
                o.size = point_size;

                return o;
            }

            half4 frag(v2f i) : SV_Target {
            	// TODO: average this texture over time? But then have issues when you move...

                UNITY_OUTPUT_DEPTH(i.depth);
            }
            ENDCG
        }
    }
}