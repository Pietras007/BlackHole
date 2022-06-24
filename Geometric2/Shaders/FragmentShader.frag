#version 430 core

const float PI = 3.14159265358979323846;
uniform samplerCube sampler;
uniform mat4 invView;
uniform vec2 resolution;

uniform float mass;
uniform vec3 blackHolePosition;

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
    float t = dot(ray.dir, point - ray.origin) / dot(ray.dir, ray.dir);
    if(t <= 0) return length(point - ray.origin);
    return length(point - (ray.origin + t * ray.dir));
}

float f(float w, float M, float b) {
    return 1 - w * w * (1 - (2 * M * w) / b);
}

float df(float w, float M, float b) {
    return -2 * w + (6 * w * w * M) / b;
}

float RootSearch(float M, float b) {
    float w = 1.0;
    float root;
    int iters = 0;
    
    while(true) {
        iters++;
        root = w - f(w, M, b) / df(w, M, b);
        
        if(abs(root - w) < 0.00001) break;
        if(iters > 1000) return -1.0;
        
        w = root;
    }
    
    return root;
}

float _pow(float w, float M, float b) {
    return pow(f(w, M, b), -0.5);
}

float Integrate(float upper, float M, float b) {
    int N = 500;
    float dx = upper / N;
    float sum = 0;
    for(int i = 0; i <= N; i++)
    {
        sum += _pow(i * dx - dx / 2, M, b);
    }

    return sum * dx;
}

vec3 RotateAngleAxis(vec3 vec, vec3 axis, float angle)
{
    float c, s;
    s = sin(angle);
    c = cos(angle);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return vec * mat3(
        t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
        t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
        t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
    );
}

void main() {
    Ray ray = GenerateRay();
    //gl_FragColor = texture(sampler, ray.dir);
    float b = PointRayDistance(blackHolePosition, ray);
    float upper = RootSearch(mass, b);
    float angle = 2 * Integrate(upper, mass, b) - PI;
    if(abs(angle) > PI || isnan(angle) || isinf(angle)) {
        gl_FragColor = vec4(0, 0, 0, 1);
    } else {
        vec3 axis = cross(normalize(vec3(0, 0, -ray.dir.z)), normalize(ray.dir));
        //vec3 axis = cross(normalize(vec3(ray.dir.x, ray.dir.y,0)), normalize(ray.dir));
        vec3 dir = normalize(RotateAngleAxis(ray.dir, -axis, angle));
        gl_FragColor = texture(sampler, dir);
    }
}