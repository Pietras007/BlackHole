#version 430 core

in vec3 position;
out vec3 textureCoords;

uniform mat4 projection;
uniform mat4 view;

void main() {
    gl_Position = vec4(position, 1.0) * view * projection;

    textureCoords = position;//vec3(0,0,0);
}