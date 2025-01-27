﻿using Herno.MIDI;
using Herno.Util;
using Herno.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herno.Components.PianoRoll.Actions
{
  public class MIDIPatternActionMoveNotes : MIDIPatternAction
  {
    public double Ticks { get; }
    public int Keys { get; }

    PerKeyArray<HashSet<int>> NoteLocations { get; set; }

    public MIDIPatternActionMoveNotes(MIDIPatternConnect pattern, IEnumerable<SelectedSNote> notesToMove, double ticks, int keys) : base(pattern)
    {
      Ticks = ticks;
      Keys = keys;

      NoteLocations = PianoRollPattern.Pattern.GetNoteLocations(notesToMove);
    }

    PerKeyArray<HashSet<int>> MoveNotes(double xOffset, int keyOffset, PerKeyArray<HashSet<int>> selectedNotes)
    {
      var pattern = PianoRollPattern.Pattern;

      var notesToMove = pattern.FetchSelectedNotes(selectedNotes);
      pattern.RemoveSelectedNotes(notesToMove);
      var shiftedNotes = notesToMove.Roll(keyOffset).MapParallel(k =>
      {
        foreach(var n in k) n.Start += xOffset;
        return k.AsEnumerable();
      });
      pattern.InjectNotes(shiftedNotes);

      var editedSelection = shiftedNotes.ToSelectedSNotes();
      PianoRollPattern.DeselectAllNotes();
      PianoRollPattern.SelectNoteRange(editedSelection);

      return pattern.GetNoteLocations(shiftedNotes);
    }

    protected override void ApplyInternal()
    {
      NoteLocations = MoveNotes(Ticks, Keys, NoteLocations);
    }

    protected override void UndoInternal()
    {
      NoteLocations = MoveNotes(-Ticks, -Keys, NoteLocations);
    }
  }
}
