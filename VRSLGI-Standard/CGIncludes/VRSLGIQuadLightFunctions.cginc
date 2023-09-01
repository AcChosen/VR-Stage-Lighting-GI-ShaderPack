    
#define SCORE_MIN -1000
int lightCount = _VRSL_LightTexture.Load( int3(0, 2, 0) ); 
const float4 worldPostiion = o.meshWorldPos;

float4 lightPositions[64];
float4 lightColors[64];
float scores[64];

float4 qLightPositions[4];
float4 qLightColors[4];

// qLightColors[0] = float4(0,0,0,0);
// qLightColors[1] = float4(0,0,0,0);
// qLightColors[2] = float4(0,0,0,0);
// qLightColors[3] = float4(0,0,0,0);
if(lightCount > 0)
{
    for (int x = 0; x < lightCount; x++)
    {
        float4 rawLightColor = _VRSL_LightTexture.Load( int3(x, 0, 0) );   
        float4 lightPos = _VRSL_LightTexture.Load( int3(x, 1, 0) );

        lightPositions[x] = lightPos;
        lightColors[x] = rawLightColor;

        scores[x] = SCORE_MIN;

        float lightIntensity = dot(rawLightColor.xyz, float3(0.2126, 0.7152, 0.0722));

        float3 objectToLight = lightPos.xyz - worldPostiion.xyz;
        float distanceSquared = dot(objectToLight, objectToLight);
        float distance = length(objectToLight);

        // float attenuationSquared = 1.0 / (1.0 + distanceSquared);
        // float attenuation = 1.0 / (1.0 + abs(distance));

        // Ideally we'd calculate a score based on the distance and intensity of the light according to an inverse law
        // but for some reason that's not working, so we'll just do this instead. It seems to work well
        float combinedScore = (-sqrt(distanceSquared) + lightIntensity);
        // float distanceScore = -distanceSquared;
        // float score = _SanctumGIPriority ? combinedScore : distanceScore;

        scores[x] = combinedScore;
    }

    int maxVal = 0;
    float3 maxPos = float3(0,0,0);
    float3 maxCol = float3(0,0,0);
    int index = 0;
    
    // Find the 4 most relevant lights based on their score
    // O(m*n), at max m = 4, n = 20, so O(80) which is not ideal but it's only per vertex, and n can be lower than 20 if many lights are off
    [unroll] for (int j = 0; j < 4; j++)
    {
        maxVal = scores[0];
        index = 0;
        [unroll] for(int i = 1; i < lightCount; i++)
        {
            if(maxVal < scores[i])
            {
                maxVal = scores[i];
                index = i;
            }
        }
        qLightPositions[j] = lightPositions[index];
        qLightColors[j] = lightColors[index];
        scores[index] = SCORE_MIN;
    }
    lightCount = min(lightCount, 4);
}