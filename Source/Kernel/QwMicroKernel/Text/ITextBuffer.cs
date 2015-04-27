﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace QwMicroKernel.Text
{
    public interface ITextBuffer
    {
        int Length { get; }
        int Position { get; set; }
        int Read();
        int Peek();
    }

    // TextBuffer with Location tracking
    public interface ITextDocument : ITextBuffer
    {
        SourceLocation Location { get; }
    }
}
