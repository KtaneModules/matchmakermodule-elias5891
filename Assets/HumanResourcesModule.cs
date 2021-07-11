using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HumanResources;
using UnityEngine;
using UnityEngine.Collections;
using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Human Resources
/// Created by Elias8885, Timwi and Skyeward
/// </summary>
public class HumanResourcesModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;

    public KMSelectable ButtonLeftNames;
    public KMSelectable ButtonRightNames;
    public KMSelectable ButtonLeftDescs;
    public KMSelectable ButtonRightDescs;
    public KMSelectable ButtonHire;
    public KMSelectable ButtonFire;

    public TextMesh NamesText;
    public TextMesh DescsText;
    private readonly TextState _nameState = new TextState();
    private readonly TextState _descState = new TextState();

    private static readonly Person[] _people =
    {
        new Person { Name = "Garuda", MBTI = "INTJ", Descriptor = "0" },
        new Person { Name = "Setra", MBTI = "INTP", Descriptor = "1" },
        new Person { Name = "Drahuska", MBTI = "INFJ", Descriptor = "2" },
        new Person { Name = "Jean", MBTI = "INFP", Descriptor = "3" },
        new Person { Name = "Draket", MBTI = "ISTJ", Descriptor = "4" },
        new Person { Name = "Burniel", MBTI = "ISTP", Descriptor = "5" },
        new Person { Name = "Depresso", MBTI = "ISFJ", Descriptor = "6" },
        new Person { Name = "Dicey", MBTI = "ISFP", Descriptor = "7" },
        new Person { Name = "SuprStep", MBTI = "ENTJ", Descriptor = "8" },
        new Person { Name = "Noah", MBTI = "ENTP", Descriptor = "9" },
        new Person { Name = "Alissa", MBTI = "ENFJ", Descriptor = "10" },
        new Person { Name = "Ashy", MBTI = "ENFP", Descriptor = "11" },
        new Person { Name = "Ghastly", MBTI = "ESTJ", Descriptor = "12" },
        new Person { Name = "Deaf", MBTI = "ESTP", Descriptor = "13" },
        new Person { Name = "Jasper", MBTI = "ESFJ", Descriptor = "14" },
        new Person { Name = "Azzaman", MBTI = "ESFP", Descriptor = "15" }
    };

    private int[] _availableNames;
    private int[] _availableDescs;

    private int[] _topNames;
    private int[] _bottomNames;

    private List<int> topNamesIP = new List<int>();
    private List<int> bottomNamesIP = new List<int>();

    private List<Person> _unusedNames = new List<Person>();

    private int _nameIndex;
    private int _descIndex;

    private int _personToFire;
    private int _personToHire;


    private int _playersselected = 0;

    private bool _correctFired = false;
    private bool _isSolved = false;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    const string _green = "48E64F";
    const string _red = "E63B5E";
    const string _solved = "BBDDFF";

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        ButtonLeftNames.OnInteract += NamesCycleLeft;
        ButtonRightNames.OnInteract += NamesCycleRight;
        ButtonLeftDescs.OnInteract += DescsCycleLeft;
        ButtonRightDescs.OnInteract += DescsCycleRight;
        ButtonHire.OnInteract += Hire;
        ButtonFire.OnInteract += Fire;

        _unusedNames = _people.ToList();

    tryAgain:

        if (_playersselected < 10)
        {

            var _pick1 = UnityEngine.Random.Range(0, _unusedNames.Count);
            var _pick2 = UnityEngine.Random.Range(0, _unusedNames.Count);

            if (_pick1 == _pick2)
                goto tryAgain;

            var _value1 = _unusedNames[_pick1].Descriptor;
            var _value2 = _unusedNames[_pick2].Descriptor;

            var _value3 = Int32.Parse(_value1);
            var _value4 = Int32.Parse(_value2);

            if (Compatible(_value3,_value4))
            {
                Debug.LogFormat("[Matchmaker #{0}]:  Players added that match:  {1} & {2}", _moduleId, _unusedNames[_pick1].Name, _unusedNames[_pick2].Name);
                topNamesIP.Add(_value3);
                bottomNamesIP.Add(_value4);
                _playersselected = _playersselected + 2;
                _unusedNames.RemoveAll(r => r.Descriptor == _value1);
                _unusedNames.RemoveAll(r => r.Descriptor == _value2);
            }
            goto tryAgain;

        }

        _availableNames = topNamesIP.Shuffle().ToArray();
        _availableDescs = bottomNamesIP.Shuffle().ToArray();
        


        
        //// Choose 10 people and 5 descriptors
        //_availableNames = Enumerable.Range(0, _people.Length).ToList().Shuffle().Take(10).ToArray();
        //_availableDescs = Enumerable.Range(0, _people.Length).ToList().Shuffle().Take(5).ToArray();

        //var personToFire = FindPerson(_availableNames.Take(5), _availableDescs.Take(3));
        //if (personToFire == null || _availableDescs.Skip(3).Contains(personToFire.PersonIndex))
        //    goto tryAgain;
        //_personToFire = personToFire.PersonIndex;

        //var personToHire = FindPerson(_availableNames.Skip(5), _availableDescs.Skip(3).Concat(new[] { _personToFire }));
        //if (personToHire == null)
        //    goto tryAgain;
        //_personToHire = personToHire.PersonIndex;

        //Debug.LogFormat("[Human Resources #{0}] Complaints: {1}", _moduleId, _availableDescs.Take(3).Select(ix => string.Format("{0} ({1})", _people[ix].Descriptor, _people[ix].MBTI)).JoinString(", "));
        //Debug.LogFormat("[Human Resources #{0}] Required: {1}, preferred: {2}", _moduleId,
        //    personToFire.Required.Length == 0 ? "(none)" : personToFire.Required.JoinString("+"),
        //    personToFire.Preferred.Length == 0 ? "(none)" : personToFire.Preferred.JoinString("+"));
        //Debug.LogFormat("[Human Resources #{0}] Employees: {1}", _moduleId, _availableNames.Take(5).Select(ix => string.Format("{0} ({1})", _people[ix].Name, _people[ix].MBTI)).JoinString(", "));
        //Debug.LogFormat("[Human Resources #{0}] Person to fire: {1} ({2})", _moduleId, _people[_personToFire].Name, _people[_personToFire].MBTI);
        //Debug.LogFormat("[Human Resources #{0}] Fired person adds desired trait: {1}", _moduleId, _people[_personToFire].Descriptor);

        //Debug.LogFormat("[Human Resources #{0}] Desired traits: {1}", _moduleId, _availableDescs.Skip(3).Concat(new[] { _personToFire }).Select(ix => string.Format("{0} ({1})", _people[ix].Descriptor, _people[ix].MBTI)).JoinString(", "));
        //Debug.LogFormat("[Human Resources #{0}] Required: {1}, preferred: {2}", _moduleId,
        //    personToHire.Required.Length == 0 ? "(none)" : personToHire.Required.JoinString("+"),
        //    personToHire.Preferred.Length == 0 ? "(none)" : personToHire.Preferred.JoinString("+"));
        //Debug.LogFormat("[Human Resources #{0}] Applicants: {1}", _moduleId, _availableNames.Skip(5).Select(ix => string.Format("{0} ({1})", _people[ix].Name, _people[ix].MBTI)).JoinString(", "));
        //Debug.LogFormat("[Human Resources #{0}] Person to hire: {1} ({2})", _moduleId, _people[_personToHire].Name, _people[_personToHire].MBTI);

        _nameIndex = Rnd.Range(0, _availableNames.Length);
        setName();
        _descIndex = Rnd.Range(0, _availableDescs.Length);
        setDesc();

        StartCoroutine(textCoroutine(NamesText, _nameState));
        StartCoroutine(textCoroutine(DescsText, _descState));
    }

    private void setName()
    {
        setText(_nameState, _people[_availableNames[_nameIndex]].Name, _nameIndex < 5 ? _green : _red);
    }

    private void setDesc()
    {
        setText(_descState, _people[_availableDescs[_descIndex]].Name, _descIndex < 5 ? _red : _green);
    }

    private void setText(TextState state, string newText, string newColor)
    {
        if (!state.CurrentlyDeleting)
        {
            state.DelText = state.InsText;
            state.DelColor = state.InsColor;
            state.CurrentlyDeleting = state.CurIndex > 0;
        }
        state.InsText = newText;
        state.InsColor = newColor;
    }

    private bool Compatible(int value1, int value2)
    {
        var score = 0;
        if ((value1 % 2) == (value2 % 2))
            score++;
        if (((value1 / 2 ) % 2) == ((value2 / 2) % 2))
            score++;
        if (((value1 / 4) % 2) == ((value2 / 4) % 2))
            score++;
        if (((value1 / 8) % 2) == ((value2 / 8) % 2))
            score++;
        if ((score == 0) || (score == 3))
            return true;
        return false;

    }
    private IEnumerator textCoroutine(TextMesh mesh, TextState state)
    {
        while (true)
        {
            yield return new WaitForSeconds(.05f);

            if (state.CurrentlyDeleting)
            {
                state.CurIndex--;
                mesh.text = string.Format("<color=#{0}>{1}</color><color=#D1D225>█</color>", state.DelColor, state.DelText.Substring(0, state.CurIndex));
                if (state.CurIndex == 0)
                    state.CurrentlyDeleting = false;
                Audio.PlaySoundAtTransform("beep_short", mesh.transform);
            }
            else if (state.CurIndex < state.InsText.Length)
            {
                state.CurIndex++;
                if (state.CurIndex < state.InsText.Length)
                    mesh.text = string.Format("<color=#{0}>{1}</color><color=#D1D225>█</color>", state.InsColor, state.InsText.Substring(0, state.CurIndex));
                else
                    mesh.text = string.Format("<color=#{0}>{1}</color>", state.InsColor, state.InsText);
                Audio.PlaySoundAtTransform("beep_short", mesh.transform);
            }
        }
    }

    private FindPersonResult FindPerson(IEnumerable<int> names, IEnumerable<int> descs)
    {
        var required = "EINSFTJP".Where(ch => descs.All(ix => _people[ix].MBTI.Contains(ch))).ToArray();
        var preferred = "EINSFTJP".Except(required).Where(ch => descs.Count(ix => _people[ix].MBTI.Contains(ch)) == 2).ToArray();
        var peopleInfos = names.Select(ix => new
        {
            Index = ix,
            RequiredCount = required.Count(ch => _people[ix].MBTI.Contains(ch)),
            PreferredCount = preferred.Count(ch => _people[ix].MBTI.Contains(ch))
        }).OrderByDescending(info => info.RequiredCount).ToArray();

        if (peopleInfos[0].RequiredCount > peopleInfos[1].RequiredCount)
            // No tie!
            return new FindPersonResult(peopleInfos[0].Index, required, preferred);

        // Number of required traits is tied; look at number of preferred traits
        var candidates = peopleInfos.Where(info => info.RequiredCount == peopleInfos[0].RequiredCount).OrderByDescending(info => info.PreferredCount).ToArray();
        if (candidates[0].PreferredCount > candidates[1].PreferredCount)
            // No tie this time!
            return new FindPersonResult(candidates[0].Index, required, preferred);

        // It’s still a tie; try again!
        return null;
    }

    private bool DescsCycleLeft()
    {
        ButtonLeftDescs.AddInteractionPunch(.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonLeftDescs.transform);

        if (_isSolved)
            return false;

        _descIndex = ((_descIndex - 1) + _availableDescs.Length) % _availableDescs.Length;
        setDesc();

        return false;
    }

    private bool DescsCycleRight()
    {
        ButtonRightDescs.AddInteractionPunch(.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonRightDescs.transform);

        if (_isSolved)
            return false;

        _descIndex = (_descIndex + 1) % _availableDescs.Length;
        setDesc();

        return false;
    }

    private bool NamesCycleLeft()
    {
        ButtonLeftNames.AddInteractionPunch(.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonLeftNames.transform);

        if (_isSolved)
            return false;

        _nameIndex = ((_nameIndex - 1) + _availableNames.Length) % _availableNames.Length;
        setName();

        return false;
    }

    private bool NamesCycleRight()
    {
        ButtonRightNames.AddInteractionPunch(.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonRightNames.transform);

        if (_isSolved)
            return false;

        _nameIndex = (_nameIndex + 1) % _availableNames.Length;
        setName();

        return false;
    }

    private bool Fire()
    {
        ButtonFire.AddInteractionPunch();
        Audio.PlaySoundAtTransform("beep_long", ButtonFire.transform);

        if (_isSolved)
            return false;

        Debug.LogFormat("[Matchmaker #{0}] Chose to match: {1} & {2}", _moduleId, _people[_availableNames[_nameIndex]].Name, _people[_availableDescs[_descIndex]].Name);

        var _value1 = Int32.Parse(_people[_availableNames[_nameIndex]].Descriptor);
        var _value2 = Int32.Parse(_people[_availableDescs[_descIndex]].Descriptor);

        if (Compatible(_value1,_value2))
        {
            Debug.LogFormat("[Matchmaker #{0}] Compatiable", _moduleId);
            if (_availableNames.Length == 1)
            {
                Debug.LogFormat("[Matchmaker #{0}] Module solved.", _moduleId);
                _isSolved = true;
                Module.HandlePass();
                setText(_nameState, "OUT", _solved);
                setText(_descState, "WITH FRIENDS", _solved);
            }
            else
            {
                var _temp1 = new List<int>(_availableNames);
                _temp1.RemoveAt(_nameIndex);
                _nameIndex = 0;
                _availableNames = _temp1.ToArray();
                var _temp2 = new List<int>(_availableDescs);
                _temp2.RemoveAt(_descIndex);
                _descIndex = 0;
                _availableDescs = _temp2.ToArray();
                NamesCycleRight();
                DescsCycleRight();
            }
        }
        else
        {
            Debug.LogFormat("[Matchmaker #{0}] Not Compatiable", _moduleId);
            Module.HandleStrike();
        }
            

        //if (_availableNames[_nameIndex] == _personToFire)
        //    _correctFired = true;
        //else
        //    Module.HandleStrike();

        return false;
    }

    private bool Hire()
    {
        //Used as a reset on this module.
        _availableNames = topNamesIP.ToArray();
        _availableDescs = bottomNamesIP.ToArray();
        
        ButtonHire.AddInteractionPunch();
        Audio.PlaySoundAtTransform("beep_long", ButtonHire.transform);

        if (_isSolved)
            return false;

        Debug.LogFormat("[Matchmaker #{0}] Choosing to reset.", _moduleId);
        return false;
    }

//#pragma warning disable 414
//    private readonly string TwitchHelpMessage = @"!{0} cycle [see all the names in both sets] | !{0} cycle top/bottom [see just one of the two] | !{0} match X Y [match X on top, Y on bottom" ;
//#pragma warning restore 414

//    private static string[] _cycleNames = { "top", "up" };
//    private static string[] _cycleDescs = { "bottom", "down" };
    //private IEnumerator ProcessTwitchCommand(string command)
    //{
    //    var pieces = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    //    if ((pieces.Length == 1 && pieces[0] == "cycle") ||
    //        (pieces.Length == 2 && pieces[0] == "cycle" && (_cycleNames.Contains(pieces[1]) || _cycleDescs.Contains(pieces[1]))))
    //    {
    //        yield return null;
    //        if (pieces.Length == 1 || _cycleNames.Contains(pieces[1]))
    //        {
    //            for (int i = 0; i < _availableNames.Length; i++)
    //            {
    //                ButtonRightNames.OnInteract();
    //                yield return "trycancel";
    //                yield return new WaitForSeconds(1.75f);
    //            }
    //        }
    //        if (pieces.Length == 1 || _cycleDescs.Contains(pieces[1]))
    //        {
    //            for (int i = 0; i < _availableDescs.Length; i++)
    //            {
    //                ButtonRightDescs.OnInteract();
    //                yield return "trycancel";
    //                yield return new WaitForSeconds(1.75f);
    //            }
    //        }
    //    }
    //    else if (pieces.Length == 2 && pieces[0] == "cycle")
    //    {
    //        yield return null;
    //        yield return "sendtochat Excuse me, cycle what now?";
    //    }
    //    else if (pieces.Length == 3 && (pieces[0] == "match"))
    //    {
    //        yield return null;
    //        for (int i = 0; i < _availableNames.Length; i++)
    //        {
    //            if (_people[_availableNames[_nameIndex]].Name.Equals(pieces[1], StringComparison.InvariantCultureIgnoreCase))
    //            {
    //                if (_people[_availableDescs[_nameIndex]].Name.Equals(pieces[1], StringComparison.InvariantCultureIgnoreCase))
    //                {
    //                    (pieces[0] == "hire" ? ButtonHire : ButtonFire).OnInteract();
    //                    yield break;
    //                }
    //            }
    //            ButtonRightNames.OnInteract();
    //            yield return new WaitForSeconds(.1f);
    //        }
    //        yield return string.Format("sendtochat Sorry, who is “{0}” again?", pieces[1]);
    //    }
    //}

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!_correctFired)
        {
            while (_availableNames[_nameIndex] != _personToFire)
            {
                ButtonRightNames.OnInteract();
                yield return new WaitForSeconds(.1f);
            }

            Debug.LogFormat(@"{0}, {1}", _nameState.CurrentlyDeleting, _nameState.InsText);
            while (_nameState.CurrentlyDeleting || _nameState.CurIndex < _nameState.InsText.Length)
                yield return true;

            ButtonFire.OnInteract();
            yield return new WaitForSeconds(.5f);
        }

        while (_availableNames[_nameIndex] != _personToHire)
        {
            ButtonRightNames.OnInteract();
            yield return new WaitForSeconds(.1f);
        }

        while (_nameState.CurrentlyDeleting || _nameState.CurIndex < _nameState.InsText.Length)
            yield return true;

        ButtonHire.OnInteract();
        yield return new WaitForSeconds(.5f);
    }
}
