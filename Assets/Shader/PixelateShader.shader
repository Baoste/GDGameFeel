Shader "Custom/PixelateShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScreenHeight("Screen Height", Float) = 64
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float _ScreenHeight;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float range = 0.02;
				float a = range / ((range - 0.5) * (range - 0.5));
				float2 newUV;
				if (uv.x < 0.5 && uv.y < 0.5)
				{
					float ty = a*(uv.x-0.5)*(uv.x-0.5);
					float tx = a*(uv.y-0.5)*(uv.y-0.5);
					if (uv.y < ty || uv.x < tx)
						newUV = 0;
					else
						newUV = uv + float2(-tx*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x > 0.5 && uv.y < 0.5)
				{
					float ty = a*(uv.x-0.5)*(uv.x-0.5);
					float tx = -a*(uv.y-0.5)*(uv.y-0.5) + 1;
					if (uv.y < ty || uv.x > tx)
						newUV = 0;
					else
						newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x < 0.5 && uv.y > 0.5)
				{
					float ty = -a*(uv.x-0.5)*(uv.x-0.5) + 1;
					float tx = a*(uv.y-0.5)*(uv.y-0.5);
					if (uv.y > ty || uv.x < tx)
						newUV = 0;
					else
						newUV = uv + float2(-tx*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}
				else
				{
					float ty = -a*(uv.x-0.5)*(uv.x-0.5) + 1;
					float tx = -a*(uv.y-0.5)*(uv.y-0.5) + 1;
					if (uv.y > ty || uv.x > tx)
						newUV = 0;
					else
						newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}
				fixed4 col = tex2D(_MainTex, newUV);
				//col.rb *= (sin(uv.y * _ScreenHeight)+1) * 0.5 + 1;
				//col.g *= (cos(uv.y * _ScreenHeight)+1) * 0.5 + 1;
				col.rgb *= step(0, sin(newUV.y * _ScreenHeight)) * 0.1 + 0.9;
				return col;
			}
			ENDCG
		}
	}
}
