namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Enigma-lite: historical rotor wirings, a reflector, a plugboard, and ring settings, with
/// simplified <i>odometer</i> stepping (the rightmost rotor steps every letter and carries
/// left) instead of the real notch/double-step mechanism. Like the real machine it is
/// <b>reciprocal</b>: because the reflector is a fixed-point-free involution and the return
/// path is the inverse of the forward path, encrypting a ciphertext with the same start
/// settings reproduces the plaintext. Non-letters pass through and do not step the rotors.
/// </summary>
public static class Enigma
{
    // Historical Enigma I rotor wirings and reflectors. Each is a permutation of A-Z.
    private static readonly IReadOnlyDictionary<string, string> Rotors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["I"] = "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
        ["II"] = "AJDKSIRUXBLHWTMCQGZNPYFVOE",
        ["III"] = "BDFHJLCPRTXVZNYEIWGAKMUSQO"
    };

    private static readonly IReadOnlyDictionary<string, string> Reflectors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["A"] = "EJMZALYXVBWFCRQUONTSPIKHGD",
        ["B"] = "YRUHQSLDPXNGOKMIEBFZCWVJAT",
        ["C"] = "FVPJIAOYEDRZXWGCTKUQSBNMHL"
    };

    public static string Encrypt(string text, EnigmaSettings settings) => Transform(text, settings);

    public static string Decrypt(string text, EnigmaSettings settings) => Transform(text, settings);

    public static string Transform(string text, EnigmaSettings settings)
    {
        ArgumentNullException.ThrowIfNull(text);
        var machine = new Machine(settings);
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (!Letters.IsAsciiLetter(c)) continue;
            chars[i] = Letters.FromIndex(machine.Encode(Letters.IndexOf(c)), Letters.IsUpper(c));
        }
        return new string(chars);
    }

    /// <summary>A single keypress's worth of machine state. Rotor positions advance as it encodes.</summary>
    private sealed class Machine
    {
        private readonly int[][] _forward;
        private readonly int[][] _backward;
        private readonly int[] _ring;
        private readonly int[] _pos;
        private readonly int[] _reflector;
        private readonly int[] _plug;
        private readonly int _count;

        public Machine(EnigmaSettings s)
        {
            ArgumentNullException.ThrowIfNull(s);
            if (s.Rotors is null || s.Rotors.Count == 0)
                throw new ArgumentException("At least one rotor is required.", nameof(s));
            if (s.Positions is null || s.Positions.Count != s.Rotors.Count)
                throw new ArgumentException("Positions count must match rotor count.", nameof(s));

            _count = s.Rotors.Count;
            _forward = new int[_count][];
            _backward = new int[_count][];
            for (int r = 0; r < _count; r++)
            {
                if (!Rotors.TryGetValue(s.Rotors[r], out var wiring))
                    throw new ArgumentException($"Unknown rotor '{s.Rotors[r]}'.", nameof(s));
                _forward[r] = wiring.Select(c => c - 'A').ToArray();
                _backward[r] = new int[Letters.AlphabetSize];
                for (int i = 0; i < Letters.AlphabetSize; i++)
                    _backward[r][_forward[r][i]] = i;
            }

            if (!Reflectors.TryGetValue(s.Reflector ?? "B", out var reflector))
                throw new ArgumentException($"Unknown reflector '{s.Reflector}'.", nameof(s));
            _reflector = reflector.Select(c => c - 'A').ToArray();

            _ring = NormalizeWheel(s.Rings, _count, nameof(s.Rings));
            _pos = s.Positions.Select(p => Letters.Mod(p, Letters.AlphabetSize)).ToArray();
            _plug = BuildPlugboard(s.Plugboard);
        }

        public int Encode(int c)
        {
            Step();
            c = _plug[c];
            for (int r = _count - 1; r >= 0; r--) c = Pass(_forward[r], r, c);
            c = _reflector[c];
            for (int r = 0; r < _count; r++) c = Pass(_backward[r], r, c);
            return _plug[c];
        }

        private int Pass(int[] wiring, int r, int c)
        {
            int shifted = Letters.Mod(c + _pos[r] - _ring[r], Letters.AlphabetSize);
            int outIndex = wiring[shifted];
            return Letters.Mod(outIndex - _pos[r] + _ring[r], Letters.AlphabetSize);
        }

        // Odometer stepping: rightmost rotor always advances; a wrap to 0 carries one rotor left.
        private void Step()
        {
            for (int r = _count - 1; r >= 0; r--)
            {
                _pos[r] = (_pos[r] + 1) % Letters.AlphabetSize;
                if (_pos[r] != 0) break;
            }
        }

        private static int[] NormalizeWheel(IReadOnlyList<int>? src, int count, string name)
        {
            if (src is null) return new int[count];
            if (src.Count != count)
                throw new ArgumentException($"{name} count must match rotor count.", name);
            return src.Select(x => Letters.Mod(x, Letters.AlphabetSize)).ToArray();
        }

        private static int[] BuildPlugboard(string? spec)
        {
            var map = new int[Letters.AlphabetSize];
            for (int i = 0; i < map.Length; i++) map[i] = i;
            if (string.IsNullOrWhiteSpace(spec)) return map;

            foreach (var pair in spec.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (pair.Length != 2 || !Letters.IsAsciiLetter(pair[0]) || !Letters.IsAsciiLetter(pair[1]))
                    throw new ArgumentException($"Plugboard pair '{pair}' must be two letters.", nameof(spec));
                int a = Letters.IndexOf(pair[0]);
                int b = Letters.IndexOf(pair[1]);
                if (a == b || map[a] != a || map[b] != b)
                    throw new ArgumentException("Each plugboard letter may be used at most once.", nameof(spec));
                map[a] = b;
                map[b] = a;
            }
            return map;
        }
    }
}
