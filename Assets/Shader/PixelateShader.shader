Shader "Custom/PixelateShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScreenHeight("Screen Height", Float) = 512
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
				float exp = 4;
				float a = range / pow((range - 0.5), exp);

				// corner twist
				float2 newUV;
				if (uv.x < 0.5 && uv.y < 0.5)
				{
					float ty = a*pow((uv.x-0.5), exp);
					float tx = a*pow((uv.y-0.5), exp);
					newUV = uv + float2(-tx*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x > 0.5 && uv.y < 0.5)
				{
					float ty = a*pow((uv.x-0.5), exp);
					float tx = -a*pow((uv.y-0.5), exp) + 1;
					newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x < 0.5 && uv.y > 0.5)
				{
					float ty = -a*pow((uv.x-0.5), exp) + 1;
					float tx = a*pow((uv.y-0.5), exp);
					newUV = uv + float2(-tx*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}
				else
				{
					float ty = -a*pow((uv.x-0.5), exp) + 1;
					float tx = -a*pow((uv.y-0.5), exp) + 1;
					if (uv.y > ty || uv.x > tx)
					newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}

				// float pixelHeight = _ScreenHeight / 3.14159265 * 2;
				// newUV.y = floor(newUV.y * pixelHeight) / pixelHeight;
				fixed4 col = tex2D(_MainTex, newUV);
				
				// scan line
				// col.rb *= step(0, sin(uv.y * _ScreenHeight)+1) * 0.5 + 1;
				// col.g *= (sin(uv.y * _ScreenHeight)+1) * 0.5 + 1;
				col.rgb *= step(0, sin(newUV.y * _ScreenHeight)) * 0.1 + 0.9;
				
				// dark corner
				float dx = newUV.x;
				float dy = newUV.y;
				float ex = 0.2;
				if (newUV.x < 0.5)
					col.rgb *= pow(dx/0.5, ex);
				else
					col.rgb *= pow((1-dx)/0.5, ex);
				if (newUV.y < 0.5)
					col.rgb *= pow(dy/0.5, ex);
				else
					col.rgb *= pow((1-dy)/0.5, ex);
				
				// black corner
				if (newUV.x < 0 || newUV.x > 1 || newUV.y < 0 || newUV.y > 1)
					col.rgb *= 0;
				return col;
			}
			ENDCG
		}
	}
}
