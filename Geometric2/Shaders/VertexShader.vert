#version 430 core

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
out vec3 textureCoords;

uniform mat4 projection;
uniform mat4 view;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;
//out mat4 invView;

void main() {

//    FragPos = vec3(a_Position);
//    Normal = vec3(aNormal);
//    TexCoords = aTexCoords;

    //invView = inverse(view * projection);

    gl_Position = vec4(a_Position, 1.0);// * view * projection;
    //textureCoords = a_Position;
}