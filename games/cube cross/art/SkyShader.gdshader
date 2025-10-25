shader_type sky;
render_mode disable_fog;

const vec3 DARK_BASE = vec3(0.05, 0.05, 0.08);

const vec3 NEON_PINK  = vec3(1.0, 0.0, 0.6);
const vec3 NEON_CYAN  = vec3(0.0, 0.8, 1.0);
const vec3 NEON_YELLOW = vec3(1.0, 0.9, 0.2);

void sky() {
    float h = clamp(EYEDIR.y * 0.5 + 0.5, 0.0, 1.0);

    float t1 = 0.5 + 0.5 * sin(TIME * 0.6);
    float t2 = 0.5 + 0.5 * sin(TIME * 0.8 + 1.0);
    float t3 = 0.5 + 0.5 * sin(TIME * 0.9 + 2.0);

    vec3 neon_mix = mix(NEON_PINK, NEON_CYAN, t1);
    neon_mix = mix(neon_mix, NEON_YELLOW, t2 * 0.5);

    vec3 col = mix(DARK_BASE, neon_mix, smoothstep(0.0, 1.0, h));

    COLOR = col;
}