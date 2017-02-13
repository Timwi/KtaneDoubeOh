using System;
using System.Collections;
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

    public KMSelectable[] Buttons;
    public GameObject Screen;
    public MeshRenderer Dot;

    private int[] _grid = @"
        60 02 15 57 36 83 48 71 24
        88 46 31 70 22 64 07 55 13
        74 27 53 05 41 18 86 30 62
        52 10 04 43 85 37 61 28 76
        33 65 78 21 00 56 12 44 87
        47 81 26 68 14 72 50 03 35
        06 38 42 84 63 20 75 17 51
        25 73 67 16 58 01 34 82 40
        11 54 80 32 77 45 23 66 08".Trim().Replace("\r", "").Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str)).ToArray();

    private int _curPos;
    private ButtonFunction[] _functions;
    private bool _isSolved;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _curPos = Enumerable.Range(0, 9 * 9).Where(i => _grid[i] >= 10).Except(new[] { 13, 37, 40, 43, 67, 39, 41, 31, 49 }).PickRandom();
        Debug.LogFormat("[Double-Oh #{1}] Start number is {0:00}.", _grid[_curPos], _moduleId);

        _functions = new ButtonFunction[5];
        _functions[0] = Rnd.Range(0, 2) == 0 ? ButtonFunction.SmallLeft : ButtonFunction.SmallRight;
        _functions[1] = Rnd.Range(0, 2) == 0 ? ButtonFunction.SmallUp : ButtonFunction.SmallDown;
        _functions[2] = Rnd.Range(0, 2) == 0 ? ButtonFunction.LargeLeft : ButtonFunction.LargeRight;
        _functions[3] = Rnd.Range(0, 2) == 0 ? ButtonFunction.LargeUp : ButtonFunction.LargeDown;
        _functions[4] = ButtonFunction.Submit;
        _functions.Shuffle();

        _isSolved = false;

        for (int i = 0; i < Buttons.Length; i++)
        {
            var j = i;
            Buttons[i].OnInteract += delegate
            {
                Buttons[j].AddInteractionPunch();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[j].transform);
                if (_isSolved)
                    return false;

                var x = _curPos % 9;
                var y = _curPos / 9;
                switch (_functions[j])
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
                Debug.LogFormat("[Double-Oh #{4}] Pressed {1}. Number is now {0:00} (grid location {2},{3}).", _grid[_curPos], _functions[j], _curPos % 9 + 1, _curPos / 9 + 1, _moduleId);
                return false;
            };
        }

        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        yield return null;

        var segments = "12"
            .Select(ch => Screen.transform.Find("Digit" + ch))
            .Select(digit => "0123456".Select(ch => digit.Find("Segment" + ch).gameObject).ToArray())
            .ToArray();

        var segmentMap = new[] { "1111101", "1001000", "0111011", "1011011", "1001110", "1010111", "1110111", "1001001", "1111111", "1011111" };

        while (true)
        {
            var num = Rnd.Range(.1f, 1f);

            var digit1 = _grid[_curPos] / 10;
            var digit2 = _grid[_curPos] % 10;
            for (int i = 0; i < 7; i++)
            {
                segments[0][i].SetActive(segmentMap[digit1][i] == '1');
                segments[1][i].SetActive(digit1 == 0 || segmentMap[digit2][i] == '1');
            }

            if (digit1 == 0)
            {
                var one = Rnd.Range(0f, 1f) * num;
                var two = Rnd.Range(0f, 1f) * num;
                segments[1][0].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
                segments[1][1].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
                segments[1][2].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, Rnd.Range(0f, 1f) * num);
                segments[1][3].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, two);
                segments[1][4].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, two);
                segments[1][5].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, Rnd.Range(0f, 1f) * num);
                segments[1][6].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, Rnd.Range(0f, 1f) * num);
                Dot.material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, one);
            }
            else
            {
                for (int i = 0; i < 7; i++)
                    segments[1][i].GetComponent<MeshRenderer>().material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, 1);
                Dot.material.color = new Color(0x43 / 255f, 0x43 / 255f, 0x43 / 255f, 1);
            }

            yield return new WaitForSeconds(.25f * (1.1f - num));
        }
    }

    private void HandleSubmit()
    {
        if (_grid[_curPos] == 0)
        {
            _isSolved = true;
            Module.HandlePass();
        }
        else if (_grid[_curPos] < 10)
        {
            Debug.LogFormat("[Double-Oh #{3}] Pressed Submit on number {0:00} (grid location {1},{2}).", _grid[_curPos], _curPos % 9 + 1, _curPos / 9 + 1, _moduleId);
            Module.HandleStrike();
        }
        else
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.Strike, Module.transform);
    }
}
