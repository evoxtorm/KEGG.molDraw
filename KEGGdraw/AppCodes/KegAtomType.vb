﻿Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Structure KegAtomType

    ''' <summary>
    ''' KCF文件之中的官能团编码
    ''' </summary>
    Dim code$
    ''' <summary>
    ''' 官能团的化学式
    ''' </summary>
    Dim formula$
    ''' <summary>
    ''' 官能团的名称
    ''' </summary>
    Dim name$
    ''' <summary>
    ''' 官能团的中心原子的类型
    ''' </summary>
    Dim type As Types

    Public ReadOnly Property FormulaValue As String
        Get
            If Not Drawable.ContainsKey(code) Then
                Return ""
            Else
                Return Drawable(code)
            End If
        End Get
    End Property

    Sub New(code$, formula$, name$, type As Types)
        Me.code = code
        Me.formula = formula
        Me.name = name
        Me.type = type
    End Sub

    Public Overrides Function ToString() As String
        Return $"{code}    {formula} / {name}"
    End Function

    Public Enum Types As Byte
        Other
        Carbon = 12
        Nitrogen = 14
        Oxygen = 16
        Sulfur = 32
        Phosphorus = 31
        ''' <summary>
        ''' 卤族元素
        ''' </summary>
        Halogen = 255
    End Enum

    ''' <summary>
    ''' <see cref="code"/>是字典的主键
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property KEGGAtomTypes As New Dictionary(Of String, KegAtomType())
    Public Shared ReadOnly Property Drawable As New Dictionary(Of String, String)

    Shared Sub New()
        With KEGGAtomTypes
            Dim blocks = My.Resources.KEGGAtomTypes _
                .lTokens _
                .Split(Function(s) s.StringEmpty, includes:=False) _
                .ToArray

            For Each part In blocks
                Dim type As Types = parseType(part(Scan0))
                Dim atoms = part _
                    .Skip(1) _
                    .Select(Function(line)
                                Dim atom As NamedValue(Of String) = line.GetTagValue(" ")
                                Dim data = Strings.Split(atom.Value, " / ")
                                Dim formula$ = "", name$

                                If data.Length = 1 Then
                                    name = data.First
                                Else
                                    formula = data.First
                                    name = data.Last
                                End If

                                Return New KegAtomType With {
                                    .code = atom.Name,
                                    .formula = formula,
                                    .name = name,
                                    .type = type
                                }
                            End Function) _
                    .GroupBy(Function(x) x.code) _
                    .ToArray

                For Each atom In atoms
                    Call .Add(atom.Key, atom.ToArray)
                Next
            Next
        End With

        With Drawable

        End With
    End Sub

    Private Shared Function parseType(s$) As Types
        s = s.Trim("+"c).Trim.Split.First
        Return [Enum].Parse(GetType(Types), s)
    End Function
End Structure
