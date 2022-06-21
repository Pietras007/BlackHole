#version 430 core

const float PI = 3.14159265358979323846;
in vec3 textureCoords;
uniform samplerCube sampler;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

void main() {
    gl_FragColor = texture(sampler, textureCoords);

}