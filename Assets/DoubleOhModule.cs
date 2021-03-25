using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubleOh;
using UnityEngine;

using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Double-Oh
/// Created by Elias, implemented by Timwi
/// </summary>
public class DoubleOhModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeedable;

    public KMSelectable[] Buttons;
    public GameObject Screen;
    public MeshRenderer Dot;
    public FakeStatusLight FakeStatusLight;

    private static readonly int[] _seed1_grid = @"
        60 02 15 57 36 83 48 71 24
        88 46 31 70 22 64 07 55 13
        74 27 53 05 41 18 86 30 62
        52 10 04 43 85 37 61 28 76
        33 65 78 21 00 56 12 44 87
        47 81 26 68 14 72 50 03 35
        06 38 42 84 63 20 75 17 51
        25 73 67 16 58 01 34 82 40
        11 54 80 32 77 45 23 66 08".Trim().Replace("\r", "").Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str)).ToArray();

    private static readonly string[] _sudokus = new[]
    {
        "abcdefghighiabcdefdefghiabcbcaefdhighigbcaefdefdhigbcacabfdeighighcabfdefdeighcab",
        "abcdefghiighabcdefdefhigabcbcaefdighhigbcaefdefdghibcacabfdehigghicabfdefdeighcab",
        "abcdefghifdehigabcighabcefdefdighcabghicabdefcabfdehighigbcafdebcaefdighdefghibca",
        "abcdefghihigabcfdeefdighabcbcafdeighghibcaefddefhigbcacabefdhigighcabdeffdeghicab",
        "abcdefghidefghiabcghiabcdeffdeighcabighcabfdecabfdeighhigbcaefdbcaefdhigefdhigbca",
        "abcdefghihgiabcdefedfghiabcbcahfgeidgiebdacfhdfhciebgacabfghideihdeabfcgfegicdhab",
        "abcdefghihigabcfdeefdighabcbcafdeighgeibhacfddhfcigbeacabhfdeigigheabdcffdegcihab",
        "abcdefghihgfabicdeedihcgabfbfaciehgddigbhaefccehgfdbiaiabedhfcggcdfabiehfheigcdab",
        "abcdefghihgfabicededigchabfbiacdgefhghebfadiccfdhiebgafabigdhceicheabfdgdegfhciab",
        "abcdefghigihabcdefdfeghiabcbcahfdeighdiegacfbegfcibhdacabidefghihdfagbcefegbchiad",
        "abcdefghigihabcdfedfegihabcbcafgeidhfhgidacebiedchbfgacabefghidegihadbcfhdfbcieag",
        "abcdefghihigabcfdeefdighabcbcafdeighdeighacfbghfcibdeacabhfgeidigheadbcffdebcihag",
        "abcdefghiiehabgcfdfdghicabebgaidefchhiefcadgbcfdghbieaeabcfdhigghfeaibdcdcibgheaf",
        "abcdefghiheiabgdfcdfgihcabebiafcehdgfhegdacibgcdhibfeaeabcfhigdigheadbcfcdfbgieah",
        "abcdefghiifhabgcdeegdhicabfhiacdbefgdcfegahibgebfhidcafaeichbgdbhigadfeccdgbfeiah",
        "abcdefghifihabgcdeedgihcafbigabfhecddhbecaigfcefgidhbahaecdbfiggfdhaibecbcifgedah",
        "abcdefghigeiabhcdffdhgciabebcaidehfghiefgcdabdgfhabieceabcidfghifdehgbcachgbfaeid",
        "abcdefghihfeaigbdcgidbchafefgaedbcihdcbghieafehifacdbgbaghfeicdiehcbdfgacdfigaheb",
        "abcdefghiidhcgbfaeefghiabcdghiabcdeffaeidhcgbbcdefghiadefghiabccgbfaeidhhiabcdefg",
        "abcdefghigdhcibafeefihgabdcihgfbdceafaeichdgbbcdeaghifceaghifbddgbafeichhifbdceag",
        "abcdefghighiabcdefdefghiabcfdehigcabcabfdehighigcabfdeighbcaefdefdighbcabcaefdigh",
        "abcdefghihigcabfdeefdighbcacabfdehigighbcaefddefghiabcbcaefdighghiabcdeffdehigcab",
        "abcdefghihigcabefdfdeighbcacabfdehigighbcadefefdghiabcbcaefdighghiabcfdedefhigcab",
        "abcdefghihigcabefdfdeighbcacabefdhigighbcafdedefghiabcbcafdeighghiabcdefefdhigcab",
        "abcdefghiighcabfdeefdhigbcagaifbcdehhdeigacfbbcfehdaigdeagfihbcchbadeigffigbchead",
        "abcdefghighiabcdefdefghiabcefdhigcabcabefdhighigcabefdighbcafdefdeighbcabcafdeigh",
        "abcdefghihdiabgecfegfchiabdcfheidbgabeagfhdicdigbcahfeicehabfdgfhdigecabgabfdcieh",
        "abcdefghieihabgfcddgfcihabefcdhgebiabeaidchgfghibfaedcidgeabcfhcfeghidabhabfcdieg",
        "abcdefghihigabcfdedefhigcabfdecabhigcabighefdighfdebcabcaghidefghiefdabcefdbcaigh",
        "abcdefghighicabdeffdeighabccabhigfdeighfdecabefdbcaighdefabchigbcaghiefdhigefdbca",
        "abcdefghiighcabdeffdeghiabccabighfdehigfdecabefdbcaighdefabchigbcahigefdghiefdbca",
        "abcdefghiighcabdeffdeghiabccabhigfdeghifdecabefdbcahigdefabcighbcaighefdhigefdbca",
        "abcdefghiighcabefdefdghiabccabighdefhigfdecabdefbcaighfdeabchigbcahigfdeghiefdbca",
        "abcdefghighicabefdefdighabciabhdgcefdghfceiabcefbiadghfieabdhcgbdaghcfiehcgefibda",
        "abcdefghighicabefdefdighabchageidbcfidefbchagbcfghaidefghadecibdeabcifghcibhfgdea",
        "abcdefghiighcabefdefdghiabcghifdecabdefbcahigcabhigdefhigefdbcafdeabcighbcaighfde",
        "abcdefghiighcabefdefdghiabcghiefdcabfdebcahigcabhigfdehigfdebcadefabcighbcaighdef",
        "abcdefghihigabcfdedefhigcabefdcabhigcabighdefighefdbcabcaghiefdghifdeabcfdebcaigh",
        "abcdefghighicabdeffdeighabccabhigefdighefdcabdefbcaighefdabchigbcaghifdehigfdebca",
        "abcdefghighicabefdefdighabchabeigdcfigefdchabdcfbhaigefdhabecigbeagcifdhcighfdbea",
        "abcdefghighicabefdefdighabciahfdgcebdgfbceiahcebhiadgfhieafdbcgfdagbchiebcgehifda",
        "abcdefghighfiabecdeidcghabfhaiecgdfbfgebdihacdcbfhaigecdhafebigieagbcfdhbfghidcea",
        "abcdefghighicabdeffdeighabcighefdcabdefbcaighcabhigefdhigfdebcaefdabchigbcaghifde",
        "abcdefghighicabdeffdeighabchigefdcabdefbcahigcabghiefdighfdebcaefdabcighbcahigfde",
        "abcdefghiighcabdeffdeghiabcghiefdcabdefbcahigcabhigefdhigfdebcaefdabcighbcaighfde",
        "abcdefghigheiabcfdifdcghabecghefdiabfdibcaegheabhigfdchigfdcbeadefabihcgbcaghedif",
        "abcdefghifdihagebceghibcafdhigbcedafdefghacibcabfdihegifdeghbcabceafdighghacibfde",
        "abcdefghighicabdeffdeighabcighefdbcadefabcighbcahigefdhigfdecabefdbcahigcabghifde",
        "abcdefghighicabdeffdeighabchigefdbcadefabchigbcaghiefdighfdecabefdbcaighcabhigfde",
        "abcdefghiighcabdeffdeghiabcghiefdbcadefabchigbcahigefdhigfdecabefdbcaighcabighfde",
        "abcdefghidefighabcghiabcdefighbcaefdfdeghicabcabfdeighhigefdbcabcahigfdeefdcabhig",
        "abcdefghidefighabcighabcfdefdebcaighghifdecabcabhigdefhigefdbcabcaghiefdefdcabhig",
        "abcdefghihifabgcdedeghicfabcdefabhiggabichefdifhgdebcabgachideffhiegdabcecdbfaigh",
        "abcdefghiighcabefdefdghiabcghiefdbcafdeabchigbcahigfdehigfdecabdefbcaighcabighdef",
        "abcdefghighibcadeffdeighbcabcahigefdighefdcabdefcabighefdabchigcabghifdehigfdeabc",
        "abcdefghifdgahibceiehbcgdafhiecabfdgdafighebccgbfdehiabcagfdiehghiebcafdefdhiacgb",
        "abcdefghiefdighabcghicabefdbcaefdighfdeghicabhigabcdefdefhigbcaighbcafdecabfdehig",
        "abcdefghifheaigbcddigbchaefidhcgbfaebcaefidghgefhadibchadibcefgcgbfdehiaefighacdb",
        "abcdefghihidagcfbegefhibacdfgebcdiahcdaghiefbbhifaedgcdagibhcefichefgbdaefbcdahig",
        "abcdefghihigabcfdedefhigcabghibcadeffdeighabccabefdhigighcabefdefdghibcabcafdeigh",
        "abcdefghifhiacgbdegdehbiafchgacfbeiddefighcabicbeadfghbahgicdefefdbhaicgcigfdehba",
        "abcdefghihidacgebfgefbhiadcegahidfcbcdbfgeiahifhcabdgebaegfchiddhiebacfgfcgidhbea",
        "abcdefghidefhigabcighabcdefcabfdehigfdeighcabghicabfdebcaghiefdefdbcaighhigefdbca",
        "abcdefghifdeighcabhigbcaefdehiabgdcfgabfdciehcfdhiebgadgfeabhicichgfdabebeachifdg",
        "abcdefghifdeighcabhigbcaefdehdaigbcfgaifbcdehcfbhdeigaigfeadhbcbchgfiadedeachbfig",
        "abcdefghiefdhigabcighabcfdecabefdhigfdeighcabghicabdefbcaghiefddefbcaighhigfdebca",
        "abcdefghidefghiabcghiabcdefeabfdgichfdgicheabicheabfdgbgahiecfdcfdbgahiehiecfdbga",
        "abcdefghifdehigcabighbcaefdhigabcdefcabfdeighefdghibcadefcabhigghiefdabcbcaighfde",
        "abcdefghifdehigcabighbcaefdhigabcfdecabefdighdefghibcaefdcabhigghifdeabcbcaighdef",
        "abcdefghiefdhigcabighbcafdehigabcefdcabfdeighdefghibcafdecabhigghiefdabcbcaighdef",
        "abcdefghidefghiabcghiabcdefeadfigbchfigbcheadbcheadfigigahdecfbcfbigahdehdecfbiga",
        "abcdefghidefhigabcighabcdefcabfdeighfdeghicabhigcabfdebcaighefdefdbcahigghiefdbca",
        "abcdefghiefdhigabcighabcfdecabfdeighdefghicabhigcabefdbcaighdeffdebcahigghiefdbca",
        "abcdefghidefghiabcghiabcdeffabidecghidecghfabcghfabidebiahfgecdecdbiahfghfgecdbia",
        "abcdefghidefghiabcghiabcdeffaeidhcgbidhcgbfaecgbfaeidhhiaefgbcdbcdhiaefgefgbcdhia",
        "abcdefghidefghicabhigcabefdcabhigdeffdebcahigighfdebcaefdighabcghiabcfdebcaefdigh",
        "abcdefghihdeaigfbcgfibchadeegafbdichicfghedabdhbcaiegfbagefchidcehidabfgfidhgbcea"
    };

    private static readonly int[][] _validPairs = new[] { new[] { 0, 2 }, new[] { 0, 4 }, new[] { 1, 2 }, new[] { 1, 4 }, new[] { 3, 4 }, new[] { 4, 18 }, new[] { 4, 24 }, new[] { 17, 18 }, new[] { 20, 21 }, new[] { 20, 22 }, new[] { 20, 23 }, new[] { 20, 24 }, new[] { 21, 25 }, new[] { 22, 25 }, new[] { 23, 25 }, new[] { 24, 25 }, new[] { 28, 29 }, new[] { 28, 30 }, new[] { 28, 31 }, new[] { 28, 32 }, new[] { 28, 33 }, new[] { 28, 34 }, new[] { 28, 35 }, new[] { 28, 36 }, new[] { 28, 38 }, new[] { 28, 39 }, new[] { 28, 40 }, new[] { 28, 42 }, new[] { 28, 43 }, new[] { 28, 44 }, new[] { 28, 47 }, new[] { 28, 48 }, new[] { 28, 49 }, new[] { 28, 53 }, new[] { 28, 54 }, new[] { 29, 37 }, new[] { 30, 37 }, new[] { 31, 37 }, new[] { 32, 37 }, new[] { 33, 37 }, new[] { 34, 37 }, new[] { 35, 37 }, new[] { 36, 37 }, new[] { 37, 38 }, new[] { 37, 39 }, new[] { 37, 40 }, new[] { 37, 41 }, new[] { 37, 42 }, new[] { 37, 43 }, new[] { 37, 44 }, new[] { 37, 45 }, new[] { 37, 46 }, new[] { 37, 47 }, new[] { 37, 48 }, new[] { 37, 49 }, new[] { 37, 50 }, new[] { 37, 51 }, new[] { 37, 53 }, new[] { 37, 54 }, new[] { 52, 53 }, new[] { 52, 54 }, new[] { 53, 55 }, new[] { 54, 55 }, new[] { 62, 63 }, new[] { 62, 64 }, new[] { 63, 65 }, new[] { 63, 71 }, new[] { 63, 72 }, new[] { 64, 65 }, new[] { 64, 71 }, new[] { 64, 72 }, new[] { 66, 67 }, new[] { 66, 68 }, new[] { 66, 69 }, new[] { 67, 70 }, new[] { 67, 73 }, new[] { 67, 74 }, new[] { 68, 70 }, new[] { 68, 73 }, new[] { 68, 74 }, new[] { 69, 70 }, new[] { 69, 73 }, new[] { 69, 74 }, new[] { 75, 76 } };

    private int[] _grid;

    private int _curPos;
    private ButtonFunction[] _functions;
    private string[] _sounds;
    private bool _isSolved;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        // RULE SEED
        var rnd = RuleSeedable.GetRNG();
        Debug.LogFormat("[Double-Oh #{0}] Using rule seed: {1}", _moduleId, rnd.Seed);
        if (rnd.Seed == 1)
            _grid = _seed1_grid;
        else
        {
            // Add extra randomness
            for (var i = rnd.Next(0, 10); i > 0; i--)
                rnd.NextDouble();

            var randomPair = _validPairs[rnd.Next(0, _validPairs.Length)].Select(ix =>
            {
                var gridInt = _sudokus[ix].Select(ch => ch - 'a').ToArray();
                var middleChar = gridInt[40];
                var numbering = rnd.ShuffleFisherYates(Enumerable.Range(1, 8).ToList());
                numbering.Insert(middleChar, 0);
                return gridInt.Select(i => numbering[i]).ToArray();
            }).ToArray();
            _grid = Enumerable.Range(0, 81).Select(ix => randomPair[0][ix] * 10 + randomPair[1][ix]).ToArray();
        }
        // END RULE SEED

        _curPos = Enumerable.Range(0, 9 * 9).Where(i => _grid[i] >= 10).Except(new[] { 13, 37, 40, 43, 67, 39, 41, 31, 49 }).PickRandom();
        Debug.LogFormat("[Double-Oh #{1}] Start number is {0:00}.", _grid[_curPos], _moduleId);

        _functions = new ButtonFunction[5];
        _functions[0] = Rnd.Range(0, 2) == 0 ? ButtonFunction.SmallLeft : ButtonFunction.SmallRight;
        _functions[1] = Rnd.Range(0, 2) == 0 ? ButtonFunction.SmallUp : ButtonFunction.SmallDown;
        _functions[2] = Rnd.Range(0, 2) == 0 ? ButtonFunction.LargeLeft : ButtonFunction.LargeRight;
        _functions[3] = Rnd.Range(0, 2) == 0 ? ButtonFunction.LargeUp : ButtonFunction.LargeDown;
        _functions[4] = ButtonFunction.Submit;
        _functions.Shuffle();

        var sounds = Enumerable.Range(1, 4).Select(i => "DoubleOPress" + i).ToList().Shuffle();
        sounds.Insert(Array.IndexOf(_functions, ButtonFunction.Submit), null);
        _sounds = sounds.ToArray();

        _isSolved = false;

        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].OnInteract += GetButtonHandler(i);
        StartCoroutine(Flicker());

        FakeStatusLight = Instantiate(FakeStatusLight);
        FakeStatusLight.GetStatusLights(transform);
        FakeStatusLight.Module = Module;
    }

    private KMSelectable.OnInteractHandler GetButtonHandler(int i)
    {
        return delegate
        {
            Buttons[i].AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[i].transform);
            if (_isSolved)
                return false;

            if (_sounds[i] != null)
                Audio.PlaySoundAtTransform(_sounds[i], Buttons[i].transform);

            var x = _curPos % 9;
            var y = _curPos / 9;
            switch (_functions[i])
            {
                case ButtonFunction.SmallLeft:
                    x = (x / 3) * 3 + (x % 3 + 2) % 3;
                    break;
                case ButtonFunction.SmallRight:
                    x = (x / 3) * 3 + (x % 3 + 1) % 3;
                    break;
                case ButtonFunction.SmallUp:
                    y = (y / 3) * 3 + (y % 3 + 2) % 3;
                    break;
                case ButtonFunction.SmallDown:
                    y = (y / 3) * 3 + (y % 3 + 1) % 3;
                    break;
                case ButtonFunction.LargeLeft:
                    x = (x + 6) % 9;
                    break;
                case ButtonFunction.LargeRight:
                    x = (x + 3) % 9;
                    break;
                case ButtonFunction.LargeUp:
                    y = (y + 6) % 9;
                    break;
                case ButtonFunction.LargeDown:
                    y = (y + 3) % 9;
                    break;

                default:    // submit button
                    HandleSubmit();
                    return false;
            }
            _curPos = y * 9 + x;
            Debug.LogFormat("[Double-Oh #{4}] Pressed {1}. Number is now {0:00} (grid location {2},{3}).", _grid[_curPos], _functions[i], _curPos % 9 + 1, _curPos / 9 + 1, _moduleId);
            return false;
        };
    }

    private IEnumerator Flicker()
    {
        yield return null;

        var random = new System.Random();
        var segments = "12"
            .Select(ch => Screen.transform.Find("Digit" + ch))
            .Select(digit => "0123456".Select(ch => digit.Find("Segment" + ch).gameObject).ToArray())
            .ToArray();

        var segmentMap = new[] { "1111101", "1001000", "0111011", "1011011", "1001110", "1010111", "1110111", "1001001", "1111111", "1011111" };

        while (true)
        {
            var num = .9 * random.NextDouble() + .1;

            var digit1 = _grid[_curPos] / 10;
            var digit2 = _grid[_curPos] % 10;
            for (int i = 0; i < 7; i++)
            {
                segments[0][i].SetActive(segmentMap[digit1][i] == '1');
                segments[1][i].SetActive(digit1 == 0 || segmentMap[digit2][i] == '1');
            }

            if (digit1 == 0)
            {
                var one = (float) (random.NextDouble() * num);
                var two = (float) (random.NextDouble() * num);
                segments[1][0].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
                segments[1][1].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
                segments[1][2].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, (float) (random.NextDouble() * num));
                segments[1][3].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, two);
                segments[1][4].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, two);
                segments[1][5].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, (float) (random.NextDouble() * num));
                segments[1][6].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, (float) (random.NextDouble() * num));
                Dot.material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
            }
            else
            {
                for (int i = 0; i < 7; i++)
                    segments[1][i].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, 1);
                Dot.material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, 1);
            }

            yield return new WaitForSeconds((float) (.25 * (1.1 - num)));
        }
    }

    private void HandleSubmit()
    {
        if (_grid[_curPos] == 0)
        {
            Debug.LogFormat("[Double-Oh #{0}] Pressed Submit on 00. Module solved.", _moduleId);
            _isSolved = true;
            FakeStatusLight.HandlePass();
            Audio.PlaySoundAtTransform("DoubleOSolve", transform);
        }
        else if (_grid[_curPos] < 10)
        {
            Debug.LogFormat("[Double-Oh #{3}] Pressed Submit on number {0:00} (grid location {1},{2}).", _grid[_curPos], _curPos % 9 + 1, _curPos / 9 + 1, _moduleId);
            FakeStatusLight.HandleStrike();
        }
        else
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.Strike, Module.transform);
            FakeStatusLight.FlashStrike();
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Cycle the buttons with “!{0} cycle”. This presses each button 3 times, in the order of vert1, horiz1, horiz2, vert2, submit. Look at whether the arrow is horizontal or vertical, and whether it has one or two lines, to see which is which. Submit your answer with “!{0} press vert1 horiz1 horiz2 vert2 submit”.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1 && parts[0].Equals("cycle", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return null;
            for (int i = 0; i < Buttons.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    yield return Buttons[i];
                    yield return new WaitForSeconds(1f);
                    yield return Buttons[i];
                }
                yield return new WaitForSeconds(.5f);
            }
        }
        else if (parts.Length > 1 && parts[0].Equals("press", StringComparison.InvariantCultureIgnoreCase))
        {
            var btns = new List<KMSelectable>();
            foreach (var part in parts.Skip(1))
            {
                if (part.Equals("horiz1", StringComparison.InvariantCultureIgnoreCase))
                    btns.Add(Buttons[1]);
                else if (part.Equals("horiz2", StringComparison.InvariantCultureIgnoreCase))
                    btns.Add(Buttons[2]);
                else if (part.Equals("vert1", StringComparison.InvariantCultureIgnoreCase))
                    btns.Add(Buttons[0]);
                else if (part.Equals("vert2", StringComparison.InvariantCultureIgnoreCase))
                    btns.Add(Buttons[3]);
                else if (part.Equals("submit", StringComparison.InvariantCultureIgnoreCase))
                    btns.Add(Buttons[4]);
                else
                    yield break;
            }

            if (btns.Count > 0)
            {
                yield return null;
                foreach (var btn in btns)
                {
                    btn.OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        int submitButton = -1;

        for (var btnIx = 0; btnIx < _functions.Length; btnIx++)
        {
            Func<bool> isCorrect;
            switch (_functions[btnIx])
            {
                case ButtonFunction.SmallLeft:
                case ButtonFunction.SmallRight:
                    isCorrect = () => _curPos % 3 == 1;
                    break;

                case ButtonFunction.LargeLeft:
                case ButtonFunction.LargeRight:
                    isCorrect = () => (_curPos / 3) % 3 == 1;
                    break;

                case ButtonFunction.SmallUp:
                case ButtonFunction.SmallDown:
                    isCorrect = () => (_curPos / 9) % 3 == 1;
                    break;

                case ButtonFunction.LargeUp:
                case ButtonFunction.LargeDown:
                    isCorrect = () => (_curPos / 27) % 3 == 1;
                    break;

                default:
                    submitButton = btnIx;
                    continue;
            }

            while (!isCorrect())
            {
                Buttons[btnIx].OnInteract();
                yield return new WaitForSeconds(.1f);
            }
        }

        Buttons[submitButton].OnInteract();
    }
}
