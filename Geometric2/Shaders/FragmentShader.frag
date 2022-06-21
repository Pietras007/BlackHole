#version 430 core

const float PI = 3.14159265358979323846;

in vec3 textureCoords;

out vec4 out_Color;

uniform samplerCube sampler;

void main() {
    vec4 abc = texture(sampler, textureCoords);
    //abc = vec4(vec3(abc), 1);
    out_Color = abc;//vec4(0.5,0.3,0.0,1);
}