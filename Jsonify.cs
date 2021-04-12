using System.Collections.Generic;
using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace GeomancerServer {
  public static class Jsonify {
    
    public static JSONObject ToJson(this Vec3 vec) {
      var obj = new JSONObject();
      obj.Add("x", vec.x);
      obj.Add("y", vec.y);
      obj.Add("z", vec.z);
      return obj;
    }

    public static JSONObject ToJson(this Vec2 vec) {
      var obj = new JSONObject();
      obj.Add("x", vec.x);
      obj.Add("y", vec.y);
      return obj;
    }
    public static JSONObject ToJson(this PatternTile obj) { 
      var json = new JSONObject();
      json.Add("shapeIndex", obj.shapeIndex);
      json.Add("rotateRadianards", obj.rotateRadianards);
      json.Add("translate", obj.translate.ToJson());
      json.Add("sideIndexToSideAdjacencies", obj.sideIndexToSideAdjacencies.ToJson());
      json.Add("cornerIndexToCornerAdjacencies", obj.cornerIndexToCornerAdjacencies.ToJson());
      return json;
    }
    public static JSONArray ToJson(this PatternSideAdjacencyImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this PatternSideAdjacency obj) {
      var json = new JSONObject();
      json.Add("groupRelativeX", obj.groupRelativeX);
      json.Add("groupRelativeY", obj.groupRelativeY);
      json.Add("tileIndex", obj.tileIndex);
      json.Add("sideIndex", obj.sideIndex);
      return json;
    }
    public static JSONArray ToJson(this PatternCornerAdjacencyImmListImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this PatternCornerAdjacencyImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this PatternCornerAdjacency obj) {
      var json = new JSONObject();
      json.Add("groupRelativeX", obj.groupRelativeX);
      json.Add("groupRelativeY", obj.groupRelativeY);
      json.Add("tileIndex", obj.tileIndex);
      json.Add("cornerIndex", obj.cornerIndex);
      return json;
    }



    public static JSONObject ToJson(this Pattern obj) {
      var json = new JSONObject();
      json.Add("name", obj.name);
      json.Add("xOffset", obj.xOffset.ToJson());
      json.Add("yOffset", obj.yOffset.ToJson());
      json.Add("shapeIndexToCorners", obj.shapeIndexToCorners.ToJson());
      json.Add("patternTiles", obj.patternTiles.ToJson());
      return json;
    }
    
    public static JSONArray ToJson(this PatternTileImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this Vec2ImmListImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this Vec2ImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this (ulong, InitialSymbol) obj) {
      var json = new JSONObject();
      json.Add("id", obj.Item1);
      json.Add("symbol", obj.Item2.ToJson());
      return json;
    }
    public static JSONArray ToJson(this List<(ulong, InitialSymbol)> obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    
    public static JSONObject ToJson(this Location obj) {
      var json = new JSONObject();
      json.Add("groupX", obj.groupX);
      json.Add("groupY", obj.groupY);
      json.Add("indexInGroup", obj.indexInGroup);
      return json;
    }
    public static JSONObject ToJson(this Vec4i obj) {
      var json = new JSONObject();
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("z", obj.z);
      json.Add("w", obj.w);
      return json;
    }
    public static JSONNode ToJson(this IVec4iAnimation anim) {
      if (anim == null) {
        return JSONNull.CreateOrGet();
      }
      if (anim is ConstantVec4iAnimation constant) {
        var json = new JSONObject();
        json.Add("type", "constant");
        json.Add("val", constant.vec.ToJson());
        return json;
      } else if (anim is AddVec4iAnimation add) {
        var json = new JSONObject();
        json.Add("type", "add");
        json.Add("left", add.left.ToJson());
        json.Add("right", add.right.ToJson());
        return json;
      } else if (anim is MultiplyVec4iAnimation multiply) {
        var json = new JSONObject();
        json.Add("type", "multiply");
        json.Add("left", multiply.left.ToJson());
        json.Add("right", multiply.right.ToJson());
        return json;
      } else if (anim is DivideVec4iAnimation divide) {
        var json = new JSONObject();
        json.Add("type", "divide");
        json.Add("left", divide.left.ToJson());
        json.Add("right", divide.right.ToJson());
        return json;
      } else {
        Asserts.Assert(false);
        return null;
      }
    }
    public static JSONNode ToJson(this SymbolId obj) {
      var json = new JSONObject();
      json.Add("fontName", obj.fontName);
      json.Add("unicode", obj.unicode);
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolGlyph obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("symbolId", obj.symbolId.ToJson());
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolOutline obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      switch (obj.mode) {
        case OutlineMode.NoOutline:
          Asserts.Assert(false);
          break;
        case OutlineMode.CenteredOutline:
          json.Add("type", "centered");
          break;
        case OutlineMode.OuterOutline:
          json.Add("type", "outer");
          break;
        default:
          Asserts.Assert(false);
          break;
      }
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolSides obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("depthPercent", obj.depthPercent);
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbol obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("glyph", obj.glyph.ToJson());
      json.Add("outline", obj.outline.ToJson());
      json.Add("sides", obj.sides.ToJson());
      json.Add("rotationDegrees", obj.rotationDegrees);
      json.Add("sizePercent", obj.sizePercent);
      return json;
    }
    public static JSONNode ToJson(this InitialUnit obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("location", obj.location.ToJson());
      json.Add("domino", obj.dominoSymbol.ToJson());
      json.Add("face", obj.faceSymbol.ToJson());
      json.Add("idToDetailSymbol", obj.idToDetailSymbol.ToJson());
      json.Add("hpRatio", obj.hpRatio);
      json.Add("mpRatio", obj.mpRatio);
      return json;
    }
    public static JSONObject ToJson(this InitialTile obj) {
      var json = new JSONObject();
      json.Add("location", obj.location.ToJson());
      json.Add("elevation", obj.elevation);
      json.Add("topColor", obj.topColor.ToJson());
      json.Add("sideColor", obj.sideColor.ToJson());
      json.Add("maybeOverlaySymbol", obj.maybeOverlaySymbol.ToJson());
      json.Add("maybeFeatureSymbol", obj.maybeFeatureSymbol.ToJson());
      json.Add("itemIdToSymbol", obj.itemIdToSymbol.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetupGameMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetupGame");
      json.Add("lookAt", obj.cameraPosition.ToJson());
      json.Add("lookAtOffsetToCamera", obj.lookatOffsetToCamera.ToJson());
      json.Add("elevationStepHeight", new JSONNumber(obj.elevationStepHeight));
      json.Add("pattern", obj.pattern.ToJson());
      return json;
    }

    public static JSONObject ToJson(this MakePanelMessage obj) {
      var json = new JSONObject();
      json.Add("command", "MakePanel");
      json.Add("id", obj.id);
      json.Add("panelGXInScreen", obj.panelGXInScreen);
      json.Add("panelGYInScreen", obj.panelGYInScreen);
      json.Add("panelGW", obj.panelGW);
      json.Add("panelGH", obj.panelGH);
      return json;
    }

    public static JSONObject ToJson(this CreateTileMessage obj) {
      var json = new JSONObject();
      json.Add("command", "CreateTile");
      json.Add("tileId", obj.newTileId);
      json.Add("initialTile", obj.initialTile.ToJson());
      return json;
    }

    public static JSONObject ToJson(this DestroyTileMessage obj) {
      var json = new JSONObject();
      json.Add("command", "DestroyTile");
      json.Add("tileId", obj.tileViewId);
      return json;
    }

    public static JSONObject ToJson(this SetSurfaceColorMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetSurfaceColor");
      json.Add("tileId", obj.tileViewId);
      json.Add("color", obj.frontColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetCliffColorMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetCliffColor");
      json.Add("tileId", obj.tileViewId);
      json.Add("color", obj.sideColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this AddRectangleMessage obj) {
      var json = new JSONObject();
      json.Add("command", "AddRectangle");
      json.Add("newViewId", obj.newViewId);
      json.Add("parentViewId", obj.parentViewId);
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("width", obj.width);
      json.Add("height", obj.height);
      json.Add("z", obj.z);
      json.Add("color", obj.color.ToJson());
      json.Add("borderColor", obj.borderColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this AddSymbolMessage obj) {
      var json = new JSONObject();
      json.Add("command", "AddSymbol");
      json.Add("newViewId", obj.newViewId);
      json.Add("parentViewId", obj.parentViewId);
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("size", obj.size);
      json.Add("z", obj.z);
      json.Add("color", obj.color.ToJson());
      json.Add("symbolId", obj.symbolId.ToJson());
      json.Add("centered", obj.centered);
      return json;
    }

    public static JSONObject ToJson(this ScheduleCloseMessage obj) {
      var json = new JSONObject();
      json.Add("command", "ScheduleClose");
      json.Add("viewId", obj.viewId);
      json.Add("startMsFromNow", obj.startMsFromNow);
      return json;
    }

    public static JSONObject ToJson(this SetElevationMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetElevation");
      json.Add("tileId", obj.tileViewId);
      json.Add("elevation", obj.elevation);
      return json;
    }

    public static JSONObject ToJson(this SetOverlayMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetOverlay");
      json.Add("tileId", obj.tileId);
      json.Add("symbol", obj.symbol.ToJson());
      return json;
    }

    public static JSONObject ToJson(this FadeIn obj) {
      var json = new JSONObject();
      json.Add("command", "SetFadeIn");
      json.Add("fadeInStartTimeMs", obj.fadeInStartTimeMs);
      json.Add("fadeInEndTimeMs", obj.fadeInEndTimeMs);
      return json;
    }

    public static JSONObject ToJson(this FadeOut obj) {
      var json = new JSONObject();
      json.Add("command", "SetFadeOut");
      json.Add("fadeOutStartTimeMs", obj.fadeOutStartTimeMs);
      json.Add("fadeOutEndTimeMs", obj.fadeOutEndTimeMs);
      return json;
    }

    public static JSONObject ToJson(this SetFadeInMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetFadeIn");
      json.Add("id", obj.id);
      json.Add("fadeIn", obj.fadeIn.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetFadeOutMessage obj) {
      var json = new JSONObject();
      json.Add("command", "SetFadeOut");
      json.Add("id", obj.id);
      json.Add("fadeOut", obj.fadeOut.ToJson());
      return json;
    }

    public static JSONObject ToJson(this RemoveViewMessage obj) {
      var json = new JSONObject();
      json.Add("command", "RemoveView");
      json.Add("viewId", obj.viewId);
      return json;
    }

    public static JSONObject ToJson(this CreateUnitMessage obj) {
      var json = new JSONObject();
      json.Add("command", "CreateUnit");
      json.Add("unitId", obj.id);
      json.Add("initialUnit", obj.initialUnit.ToJson());
      return json;
    }

    public static JSONObject ToJson(this DestroyUnitMessage obj) {
      var json = new JSONObject();
      json.Add("command", "DestroyUnit");
      json.Add("unitId", obj.unitId);
      return json;
    }

    public static JSONObject ToJson(this IDominoMessage command) {
      if (command is SetupGameMessage setupGame) {
        return setupGame.ToJson();
      } else if (command is MakePanelMessage makePanel) {
        return makePanel.ToJson();
      } else if (command is CreateTileMessage createTile) {
        return createTile.ToJson();
      } else if (command is DestroyTileMessage destroyTile) {
        return destroyTile.ToJson();
      } else if (command is SetSurfaceColorMessage setSurfaceColor) {
        return setSurfaceColor.ToJson();
      } else if (command is SetCliffColorMessage setCliffColor) {
        return setCliffColor.ToJson();
      } else if (command is AddRectangleMessage addRectangle) {
        return addRectangle.ToJson();
      } else if (command is AddSymbolMessage addSymbol) {
        return addSymbol.ToJson();
      } else if (command is ScheduleCloseMessage scheduleClose) {
        return scheduleClose.ToJson();
      } else if (command is SetElevationMessage setElevation) {
        return setElevation.ToJson();
      } else if (command is SetOverlayMessage setOverlay) {
        return setOverlay.ToJson();
      } else if (command is SetFadeInMessage setFadeIn) {
        return setFadeIn.ToJson();
      } else if (command is SetFadeOutMessage setFadeOut) {
        return setFadeOut.ToJson();
      } else if (command is RemoveViewMessage removeView) {
        return removeView.ToJson();
      } else if (command is CreateUnitMessage createUnit) {
        return createUnit.ToJson();
      } else if (command is DestroyUnitMessage destroyUnit) {
        return destroyUnit.ToJson();
      } else {
        Asserts.Assert(false);
        return null;
      }
    }
  }
}