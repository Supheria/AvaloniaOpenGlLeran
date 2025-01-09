using System;
using System.Numerics;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using static Avalonia.OpenGL.GlConsts;
using static Common.GlConstExtensions;

namespace AvaloniaOpenGlLeran.Controls;

sealed unsafe class CustomOpenGlControl : OpenGlControlBase
{
    string VertexShaderSource =>
        @"#version 300 es
                in vec3 Position;

                void main()
                {
                    gl_Position = vec4(Position.x, Position.y, Position.z, 1.0);
                }
            ";
    string FragmentShaderSource =>
        @"#version 300 es 
                precision mediump float;

                out vec4 FragColor;
              
                void main()
                {
                    FragColor = vec4(1, 0, 1, 1);
                } 
            ";

    int _vbo;
    int _vao;
    int _vertexShader;
    int _fragmentShader;
    int _shaderProgram;
    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);

        ConfigureShaders(gl);
        CreateVertexBuffer(gl);

        gl.CheckError();
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);

        gl.BindBuffer(GL_ARRAY_BUFFER, 0);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);
        gl.UseProgram(0);

        gl.DeleteBuffer(_vbo);
        gl.DeleteVertexArray(_vao);
        gl.DeleteProgram(_shaderProgram);
        gl.DeleteShader(_fragmentShader);
        gl.DeleteShader(_vertexShader);

        gl.CheckError();
    }

    void ConfigureShaders(GlInterface gl)
    {
        _shaderProgram = gl.CreateProgram();

        CreateVertexShader(gl);
        CreateFragmentShader(gl);

        Console.WriteLine(gl.LinkProgramAndGetError(_shaderProgram));

        gl.UseProgram(_shaderProgram);
    }

    void CreateFragmentShader(GlInterface gl)
    {
        _fragmentShader = gl.CreateShader(GL_FRAGMENT_SHADER);
        Console.WriteLine(gl.CompileShaderAndGetError(_fragmentShader, FragmentShaderSource));
        gl.AttachShader(_shaderProgram, _fragmentShader);
    }

    void CreateVertexShader(GlInterface gl)
    {
        _vertexShader = gl.CreateShader(GL_VERTEX_SHADER);
        Console.WriteLine(gl.CompileShaderAndGetError(_vertexShader, VertexShaderSource));
        gl.AttachShader(_shaderProgram, _vertexShader);
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        gl.ClearColor(0, 0, 0, 1);
        gl.Clear(GL_COLOR_BUFFER_BIT);

        gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

        gl.DrawArrays(GL_TRIANGLES, 0, new IntPtr(3));
        gl.CheckError();
    }

    void CreateVertexBuffer(GlInterface gl)
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, -1f, 0.0f),
            new Vector3(1f, -1f, 0.0f),
            new Vector3(0.0f, 1f, 0.0f),
        };

        _vbo = gl.GenBuffer();
        gl.BindBuffer(GL_ARRAY_BUFFER, _vbo);

        fixed (void* pVertices = vertices)
            gl.BufferData(
                GL_ARRAY_BUFFER,
                new IntPtr(sizeof(Vector3) * vertices.Length),
                new IntPtr(pVertices),
                GL_STATIC_DRAW
            );

        _vao = gl.GenVertexArray();
        gl.BindVertexArray(_vao);

        gl.VertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vector3), IntPtr.Zero);
        gl.EnableVertexAttribArray(0);
    }
}
