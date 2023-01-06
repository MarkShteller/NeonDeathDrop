// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Blend Add Shader (as used in Diablo 3)
// Uses the alpha channel to determine if the pixel needs to be blended additively or by transparency.
// Is a good way prevent the additive buildup that makes a scene with a lot of particle effects white and unreadable while still having some particle texture features.
// Idea by Julian Love - http://www.gdcvault.com/play/1017660/Technical-Artist-Bootcamp-The-VFX
Shader "Custom/Blend Add Particle Test"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_BlendThreshold("Blend Treshold (0.0:Additive, 1.0:Trasparency)", Range(0.0, 1.0)) = 0.5
	}

		SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			Lighting Off
			Fog{ Mode Off }
			ZWrite Off
			Cull Off

			// Alpha Channel: White = Transparent blend; Black = Additive blend
			Pass
			{
			// Additive blending
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _BlendThreshold;

			// Struct Input || VertOut
			struct appdata {
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			//VertIn
			struct v2f {
				half4 pos : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}


			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;
				fixed4 tex = tex2D(_MainTex, i.texcoord);

				if (tex.a >= _BlendThreshold) discard;
				col.rgb = i.color.rgb * tex.rgb;
				col.a = i.color.a * tex.a;
				return col;
			}
			ENDCG
		}

		Pass
		{
				// Alpha blending
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _BlendThreshold;

				// Struct Input || VertOut
				struct appdata {
					half4 vertex : POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				//VertIn
				struct v2f {
					half4 pos : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					return o;
				}


				fixed4 frag(v2f i) : COLOR
				{
					fixed4 col;
					fixed4 tex = tex2D(_MainTex, i.texcoord);

					if (tex.a < _BlendThreshold) discard;
					col.rgb = i.color.rgb * tex.rgb;
					col.a = i.color.a * tex.a;
					return col;
				}
				ENDCG
			}
		}
			FallBack "Diffuse"
}