#version 430 core

const float PI = 3.14159265358979323846;
in vec3 textureCoords;
uniform samplerCube sampler;
uniform mat4 invView;
uniform vec2 resolution;

uniform float mass;
uniform vec3 blackHolePosition;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

struct Ray {
    vec3 origin;
    vec3 destination;
    vec3 dir;
};

Ray GenerateRay() {
    Ray ray;
    vec2 xy = (2.0 * gl_FragCoord.xy) / resolution - 1.0;
    vec4 from = vec4(xy, -1, 1) * invView;
    vec4 to = vec4(xy, 1,1) * invView;
       from /= from.w;
    to /= to.w;
    ray.origin = from.xyz;
    ray.destination = to.xyz;
    ray.dir = normalize(to.xyz - from.xyz);
    return ray;
}

float PointRayDistance(vec3 point, Ray ray) {
    float t = dot(ray.dir, - ray.origin) / dot(ray.dir, ray.dir);
    if(t <= 0) return length(- ray.origin);
    return length(- (ray.origin + t * ray.dir));
}

float f(float x, float M, float b) {
    return 1 - x * x * (1 - 2 * x * M / b);
}

float df(float x, float M, float b) {
    return 2 * x * (3 * M * x / b - 1);
}

float g(float x, float M, float b) {
    return pow(f(x, M, b), -0.5);
}

float RootSearch(float M, float b) {
    float x = 1.0;
    float root;
    int iters = 0;
    
    while(true) {
        iters++;
        root = x - f(x, M, b) / df(x, M, b);
        
        if(abs(root - x) < 0.00001) break;
        if(iters > 1000) return -1.0;
        
        x = root;
    }
    
    return root;
}

float Integrate(float upper, float M, float b) {
    int N = 1024;
    float dx = upper / N;
    float sum = 0;
    for(int i = 1; i < N; i++) sum += g(i * dx - dx / 2, M, b);
    return sum * dx;
}

mat4 RotationMatrix(vec3 axis, float angle) {
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;

    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
    oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
    oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
    0.0,                                0.0,                                0.0,                                1.0);
}

vec3 Rotate(vec3 vec, vec3 axis, float angle) {
    mat4 rot = RotationMatrix(axis, angle);
    return vec3( vec4(vec, 0) * rot);
}

void main() {
    Ray ray = GenerateRay();
    float b = PointRayDistance(blackHolePosition, ray);
    if(b == -1) gl_FragColor = texture(sampler, ray.dir);
    else {
        float upper = RootSearch(mass, b);
        if(upper < 0) {
            gl_FragColor = vec4(0, 0, 0, 1);
            return;
        }
        float angle = 2 * Integrate(upper, mass, b) - PI;
        if(abs(angle) > PI || isnan(angle) || isinf(angle)) {
            gl_FragColor = vec4(0, 0, 0, 1);
        } else {
            vec3 axis = cross(normalize(vec3(ray.dir.x, -ray.dir.y, 0)), normalize(ray.dir));
            vec3 dir = normalize(Rotate(ray.dir, axis, angle));
            gl_FragColor = texture(sampler, dir);
        }
    }
    //gl_FragColor = texture(sampler, ray.dir);

}