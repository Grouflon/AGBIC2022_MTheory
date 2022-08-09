Shader "Unlit/BoardNodes"
{
    Properties
    {
        _bgColor("Background color", Color) = (.25, .5, .5, 1)
        _nodeColor("Node color", Color) = (.0, .0, .0, 1)
        _linkWidth("Link width", float) = .1
        _nodeRadius("Node radius", float) = .2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            // parameters
            float4 _bgColor;
            float4 _nodeColor;
            float _linkWidth;
            float _nodeRadius;

            float4 _nodes[16];
            int _nodeCount;

            float4 _links[16];
            int _linkCount;

            // constants
            static const int SIDE = 3;
            static const float CELL_SIZE = 1.f / SIDE;
            static const float HALF_CELL_SIZE = CELL_SIZE * .5;

            // functions
            float sdfCircle(float2 _v, float2 _origin, float _radius)
            {
                return length(_v - _origin) - _radius;
            }

            float sdfSegment(float2 _v, float2 _a, float2 _b, float _radius)
            {
            float2 va = _v - _a;
            float2 ba = _b - _a;
            float h = clamp(dot(va,ba)/dot(ba,ba), 0.0, 1.0 );
            return length(va - ba*h) - _radius;
            }

            float2 cellIndexToCoord(int _i)
            {
                int x = _i % SIDE;
                return float2(
                    x,
                    (_i - x) / SIDE
                );
            }

            float2 computeCellCenter(int _i)
            {
                float2 coord = cellIndexToCoord(_i);
                return coord * CELL_SIZE + float2(HALF_CELL_SIZE, HALF_CELL_SIZE);
            }

            // shaders
            v2f vert (appdata _v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(_v.vertex);
                o.uv = _v.uv;
                return o;
            }

            fixed4 frag (v2f _i) : SV_Target
            {
                float s = 0.;
                for (int i = 0; i < _nodeCount; ++i)
                {
                    float2 n1 = computeCellCenter(_nodes[i].x);

                    s += step(sdfCircle(_i.uv, n1, _nodeRadius), 0);
                }

                for (int i = 0; i < _linkCount; ++i)
                {
                    float2 n1 = computeCellCenter(_links[i].x);
                    float2 n2 = computeCellCenter(_links[i].y);

                    s += (1. - s) * step(sdfSegment(_i.uv, n1, n2, _linkWidth), 0);
                }

                return s * _nodeColor + (1. - s) * _bgColor;
            }
            ENDCG
        }
    }
}
