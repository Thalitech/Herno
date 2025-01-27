﻿using ImGuiNET;
using Herno.Components.PianoRoll;
using Herno.MIDI;
using Herno.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Herno.Components
{
  // Interaction state machine
  interface IPianoRollInteraction
  {
    // Next interaction to use. If NextInteraction is set, then this interaction is ended.
    bool IsEnded => NextInteraction != null;
    IPianoRollInteraction? NextInteraction { get; }
    void Act();
  }



  interface IPianoRollPatternInteraction : IPianoRollInteraction
  {
    MIDIPatternConnect PianoRollPattern { get; }
  }



  abstract class MIDIPatternInteraction : IPianoRollPatternInteraction
  {
    protected MIDIPatternInteraction(MIDIPatternConnect pianoRollPattern)
    {
      PianoRollPattern = pianoRollPattern;
    }

    public MIDIPatternConnect PianoRollPattern { get; }

    public IPianoRollInteraction? NextInteraction { get; private set; }


    public void Act()
    {
      var next = DoInteraction();
      if(next != null)
      {
        NextInteraction = next;
      }
    }

    protected void ContinueWith(IPianoRollInteraction? interaction)
    {
      NextInteraction = interaction;
    }

    public virtual IPianoRollInteraction? DoInteraction()
    {
      HandleZoomInteraction();
      return null;
    }

    protected void HandleZoomInteraction()
    {
      var io = ImGui.GetIO();
      var mouseRelativePos = GetMouseOutsidePos();

      if(ImGui.IsItemHovered())
      {
        if(io.MouseWheel != 0)
        {
          var zoom = Math.Pow(1.2, -io.MouseWheel);

          var viewFrame = PianoRollPattern.ViewFrame;

          if(io.KeyCtrl)
          {
            viewFrame.ZoomHorizontalAt(mouseRelativePos.X, zoom);
          }
          else if(io.KeyAlt)
          {
            viewFrame.ZoomVerticalAt(mouseRelativePos.Y, zoom);
          }
          else if(io.KeyShift)
          {
            viewFrame.ScrollHorizontalBy(viewFrame.TrueWidth / 8 * -io.MouseWheel);
          }
          else
          {
            viewFrame.ScrollVerticalBy(viewFrame.TrueHeight / 8 * -io.MouseWheel);
          }
        }
      }
    }

    protected Vector2d GetMouseOutsidePos()
    {
      var pixelPos = ImGui.GetMousePos() - ImGui.GetWindowPos() - ImGui.GetCursorStartPos();
      var relativePos = pixelPos / ImGui.GetItemRectSize();
      return relativePos.ToDoubleVec();
    }

    protected Vector2d GetMousePos()
    {
      return PianoRollPattern.GetPositionInside(GetMouseOutsidePos());
    }
  }
}
