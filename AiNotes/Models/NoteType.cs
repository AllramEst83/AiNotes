﻿using System.ComponentModel;

namespace AiNotes.Models
{
    public enum NoteType
    {
        [Description("None")]
        None = 0,
        [Description("Fritt antecknande")]
        FrittAntecknande,
        [Description("Samtal, 2 personer")]
        Samtal2Personer,
        [Description("Möte, flera personer")]
        MoteFleraPersoner,
        [Description("Lista")]
        Lista
    }
}
