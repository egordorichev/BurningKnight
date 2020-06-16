using System;

namespace BurningKnight.util {
  public static class LoadScreenTips {
    public static string Generate() {
      return jokes[new Random().Next(jokes.Length)];
    }

    private static readonly string[] jokes = {
      "[cl yellow]Maanex[cl] holds the secret to looping",
      "[cl red]Redkey[cl] is the key",
      "Crosses on the walls mean something...",
      "Press R for a surprise!",
      "Not all paintings are useless",
      "Aiming at enemies helps to hit them"
    };
  }
}