using System;
using System.Collections.Generic;
using Geomancer;
using Geomancer.Model;
using SimpleJSON;

namespace Domino {

  public class GameToDominoConnection {
    public delegate void IEventHandler();
    public interface IEvent {}
    public class Event : IEvent, IDisposable {
      public ulong id { get; private set; }
      public IEventHandler handler { get; private set; }
      public Event(ulong id, IEventHandler handler) {
        this.id = id;
        this.handler = handler;
      }
      public void Dispose() {
        Asserts.Assert(handler != null);
        id = 0;
        handler = null;
      }
      public void Trigger() {
        Asserts.Assert(handler != null);
        handler();
      }
    }

    // public DominoToGameConnection otherSide;
    // public EditorServer server;

    private List<IDominoMessage> messages = new List<IDominoMessage>();
    private ulong nextId = 1;

    private Dictionary<ulong, Event> events;

    public GameToDominoConnection() {
      events = new Dictionary<ulong, Event>();
      // this.otherSide = otherSide;
      // server = new EditorServer(this);
    }

    public ulong MakePanel(
        int panelGXInScreen,
        int panelGYInScreen,
        int panelGW,
        int panelGH) {
      ulong id = nextId++;
      messages.Add(new MakePanelMessage(id, panelGXInScreen, panelGYInScreen, panelGW, panelGH));
      return id;
      // return new Panel(this, id, panelGW, panelGH);
    }

    public IEvent MakeEvent(IEventHandler handler) {
      var id = nextId++;
      var e = new Event(id, handler);
      events.Add(id, e);
      return e;
    }
    public Event DestroyEvent(IEvent ie) {
      var e = ie as Event;
      Asserts.Assert(e != null);
      events.Remove(e.id);
      e.Dispose();
      return e;
    }

    public void TriggerEvent(ulong id) {
      if (events.TryGetValue(id, out var e)) {
        e.Trigger();
      } else {
        throw new Exception("Unknown event triggered: " + id);
      }
    }

    public ulong CreateTile(InitialTile initialTile) {
      ulong id = nextId++;
      messages.Add(new CreateTileMessage(id, initialTile));
      return id;
    }

    public ulong CreateUnit(InitialUnit initialUnit) {
      ulong id = nextId++;
      messages.Add(new CreateUnitMessage(id, initialUnit));
      return id;
    }

    public void SetupGame(Vec3 cameraPosition, Vec3 lookatOffsetToCamera, int elevationStepHeight, Pattern pattern) {
      messages.Add(new SetupGameMessage(cameraPosition, lookatOffsetToCamera, elevationStepHeight, pattern));
    }

    public void ScheduleClose(ulong viewId, long startMsFromNow) {
      messages.Add(new ScheduleCloseMessage(viewId, startMsFromNow));
    }

    public void RemoveView(ulong viewId) {
      messages.Add(new RemoveViewMessage(viewId));
    }

    public void SetOpacity(ulong viewId, int id, int percent) {
      messages.Add(new SetOpacityMessage(viewId, id, percent));
    }

    public void SetFadeOut(ulong id, FadeOut fadeOut) {
      messages.Add(new SetFadeOutMessage(id, fadeOut));
    }

    public void SetFadeIn(ulong id, FadeIn fadeIn) {
      messages.Add(new SetFadeInMessage(id, fadeIn));
    }

    public List<ulong> AddString(
        ulong parentViewId,
        int x,
        int y,
        int maxWide,
        Vec4i color,
        string fontName,
        string str) {
      List<ulong> newViewIds = new List<ulong>();
      for (int i = 0; i < str.Length; i++) {
        newViewIds.Add(AddSymbol(parentViewId, x + i, y, 1, 1, color, new SymbolId(fontName, char.ConvertToUtf32(str[i].ToString(), 0)), true));
      }
      return newViewIds;
    }

    public ulong AddButton(
        ulong parentViewId,
        int x,
        int y,
        int width,
        int height,
        int z,
        Vec4i color,
        Vec4i borderColor,
        Vec4i pressedColor,
        IEvent onClickedI,
        IEvent onMouseInI,
        IEvent onMouseOutI) {
      var onClicked = onClickedI as Event;
      Asserts.Assert(onClicked != null);
      var onMouseIn = onMouseInI as Event;
      Asserts.Assert(onMouseIn != null);
      var onMouseOut = onMouseOutI as Event;
      Asserts.Assert(onMouseOut != null);
      
      ulong newViewId = nextId++;
      messages.Add(
          new AddButtonMessage(
              newViewId, parentViewId, x, y, width, height, z, color, borderColor, pressedColor, onClicked.id,
              onMouseIn.id, onMouseOut.id));
      return newViewId;
    }

    // public ulong AddFullscreenRect(ulong parentViewId, Color color) {
    //   ulong newViewId = nextId++;
    //   messages.Add(new AddFullscreenRectMessage(newViewId, parentViewId, color));
    //   return newViewId;
    // }

    public ulong AddRectangle(
        ulong parentViewId,
        int x,
        int y,
        int width,
        int height,
        int z,
        Vec4i color,
        Vec4i borderColor) {
      ulong newViewId = nextId++;
      messages.Add(
          new AddRectangleMessage(newViewId, parentViewId, x, y, width, height, z, color, borderColor));
      return newViewId;
    }

    public ulong AddSymbol(
        ulong parentViewId,
        int x,
        int y,
        int size,
        int z,
        Vec4i color,
        SymbolId symbol,
        bool centered) {
      ulong newViewId = nextId++;
      messages.Add(new AddSymbolMessage(newViewId, parentViewId, x, y, size, z, color, symbol, centered));
      return newViewId;
    }
    
    
    public void ShowPrism(ulong tileViewId, InitialSymbol prismDescription, InitialSymbol prismOverlayDescription) {
      messages.Add(new ShowPrismMessage(tileViewId, prismDescription, prismOverlayDescription));
    }
    public void FadeInThenOut(ulong tileViewId, long inDurationMs, long outDurationMs) {
      messages.Add(new FadeInThenOutMessage(tileViewId, inDurationMs, outDurationMs));
    }
    public void ShowRune(ulong tileViewId, InitialSymbol runeSymbolDescription) {
      messages.Add(new ShowRuneMessage(tileViewId, runeSymbolDescription));
    }
    public void SetOverlay(ulong tileViewId, InitialSymbol maybeOverlay) {
      messages.Add(new SetOverlayMessage(tileViewId, maybeOverlay));
    }
    public void SetFeature(ulong tileViewId, InitialSymbol maybeFeature) {
      messages.Add(new SetFeatureMessage(tileViewId, maybeFeature));
    }
    public void SetCliffColor(ulong tileViewId, IVec4iAnimation sideColor) {
      messages.Add(new SetCliffColorMessage(tileViewId, sideColor));
    }
    public void SetSurfaceColor(ulong tileViewId, IVec4iAnimation frontColor) {
      messages.Add(new SetSurfaceColorMessage(tileViewId, frontColor));
    }
    public void SetElevation(ulong tileViewId, int elevation) {
      messages.Add(new SetElevationMessage(tileViewId, elevation));
    }
    public void RemoveItem(ulong tileViewId, ulong id) {
      messages.Add(new RemoveItemMessage(tileViewId, id));
    }
    public void ClearItems(ulong tileViewId) {
      messages.Add(new ClearItemsMessage(tileViewId));
    }
    public void AddItem(ulong tileViewId, ulong itemId, InitialSymbol symbolDescription) {
      messages.Add(new AddItemMessage(tileViewId, itemId, symbolDescription));
    }
    public void DestroyTile(ulong tileViewId) {
      messages.Add(new DestroyTileMessage(tileViewId));
    }

    public void DestroyUnit(ulong unitViewId) {
      messages.Add(new DestroyUnitMessage(unitViewId));
    }

    public List<IDominoMessage> TakeMessages() {
      var copy = new List<IDominoMessage>(messages);
      messages.Clear();
      return copy;
    }
  }
}
