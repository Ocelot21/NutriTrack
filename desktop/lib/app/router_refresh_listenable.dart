import 'dart:async';

import 'package:flutter/foundation.dart';

/// A simple GoRouter refresh listenable.
///
/// Call [ping] to notify listeners.
class RouterRefreshListenable extends ChangeNotifier {
  void ping() => notifyListeners();

  /// Utility: turn a stream into a refresh trigger.
  StreamSubscription<T> listenTo<T>(Stream<T> stream) {
    return stream.listen((_) => ping());
  }
}

