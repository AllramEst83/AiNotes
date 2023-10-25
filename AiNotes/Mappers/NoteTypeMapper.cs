using AiNotes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AiNotes.Mappers
{
    public static class NoteTypeMapper
    {
        public static string GetDescription(string noteTypeName)
        {
            NoteType noteType;

            if (!Enum.TryParse(noteTypeName, out noteType))
            {
                throw new ArgumentException("Invalid note type name", nameof(noteTypeName));
            }

            return GetInstruction(noteType);
        }

        public static string GetInstruction(NoteType noteType)
        {
            switch (noteType)
            {
                case NoteType.FrittAntecknande:
                    return "Instruction: \"Vänligen kategorisera följande innehåll under en H3-rubrik och lista de viktigaste punkterna i en punkt- eller numrerad format baserat på den logiska progressionen eller betydelsen av objekten.\"";
                case NoteType.Samtal2Personer:
                    return "Instruction: \"Granska samtalet mellan de två individerna och sammanfatta de viktigaste diskussionspunkterna, överenskommelserna och meningsskiljaktigheterna på ett sammanhängande sätt. Använd en numrerad lista för att organisera informationen på ett visuellt tilltalande sätt.\"";
                case NoteType.MoteFleraPersoner:
                    return "Instruction: \"Granska mötesanteckningarna från teammötet och sammanfatta diskussionerna, överenskommelserna, meningsskiljaktigheterna och deklarationerna som gjordes under mötet. Organisera sammanfattningen på ett klart och visuellt tilltalande sätt, med hjälp av punktlistor eller numrerade listor för att markera de viktigaste punkterna.\"";
                case NoteType.Lista:
                    return "Instruction: \"Skapa en Sammanfattad Lista av varor som nämns.Läsa noga igenom en given text och identifiera alla de varor som nämns. Målet är att utarbeta en översiktlig och koncis lista över dessa\"";
                default:
                    throw new ArgumentOutOfRangeException(nameof(noteType), noteType, null);
            }
        }
    }
}
