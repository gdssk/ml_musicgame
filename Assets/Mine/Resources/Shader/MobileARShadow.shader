Shader "Custom/MobileARShadow"
{
    SubShader {
        Pass {
            Tags { "LightMode" = "ForwardBase" "RenderType"="Opaque" "Queue"="Geometry+1" "ForceNoShadowCasting"="True"  }
	        LOD 150
        
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc" 
            #pragma multi_compile_fwdbase
            #include "AutoLight.cginc"
  
            struct appdata
            {
                float4 vertex : POSITION;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
  
            struct v2f
            {
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(0,1)
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
  
            v2f vert(appdata v) {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos (v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);                
                return o;
            }
 
            fixed4 frag(v2f i) : COLOR {
            
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            
                float attenuation = LIGHT_ATTENUATION(i);    
                attenuation = (attenuation*.5+.5);
                
                if (attenuation >= 1)
                {
                    return 0;
                }
                else
                {
                    return fixed4(.2,.2,.2,1);
                }  
            }
 
            ENDCG
        }
    }
    Fallback "VertexLit"

}