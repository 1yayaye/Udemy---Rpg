mergeInto(LibraryManager.library, {
  ShowcaseSubmitScore: function (payloadPtr) {
    var payload = UTF8ToString(payloadPtr);
    if (typeof window === 'undefined' || !window.ShowcaseUnityBridge) {
      return;
    }
    window.ShowcaseUnityBridge.submitScore(payload).catch(function (error) {
      console.error('Showcase submitScore failed', error);
    });
  },

  ShowcaseSaveGame: function (payloadPtr) {
    var payload = UTF8ToString(payloadPtr);
    if (typeof window === 'undefined' || !window.ShowcaseUnityBridge) {
      return;
    }
    window.ShowcaseUnityBridge.saveGame(payload).catch(function (error) {
      console.error('Showcase saveGame failed', error);
    });
  },

  ShowcaseUpdateGameState: function (payloadPtr) {
    var payload = UTF8ToString(payloadPtr);
    if (typeof window === 'undefined' || !window.ShowcaseUnityBridge || !window.ShowcaseUnityBridge.updateGameState) {
      return;
    }
    window.ShowcaseUnityBridge.updateGameState(payload);
  },

  ShowcaseUnityReady: function () {
    if (typeof window === 'undefined' || !window.ShowcaseUnityBridge) {
      return;
    }
    window.ShowcaseUnityBridge.unityReady();
  },

  ShowcaseUnityError: function (messagePtr) {
    var message = UTF8ToString(messagePtr);
    if (typeof window === 'undefined' || !window.ShowcaseUnityBridge) {
      return;
    }
    window.ShowcaseUnityBridge.unityError(message);
  }
});
