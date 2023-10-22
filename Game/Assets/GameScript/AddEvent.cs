using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum MotionType {FOLLOW, FIX}
public enum ScreenEffect { NONE, SCREEN_SHAKE, PUMP}
//public enum Interpolation { NONE, MOVETO, EXP, LAGACY}
public enum EventTimeAnimation{DURATION, KEY}
public class AddEvent : MonoBehaviour
{
    /*
    public Vector3 camera_position;
    public float camera_rotation;
    public float camera_scale =5F;
    public float duration=0.1F;
    public Interpolation interpole;
    public MotionType motion;
    public float speed=1F;
    public Interpolation interpoleColor;
    public float duration_color = 0.5F;
    public Color ColorA = new Color(0,0,0);
    public Color ColorB = new Color(1, 1, 1);
    public ScreenEffect screen=ScreenEffect.NONE;
    public bool showPath = false;
    public float amplitude = 1F; 
    public EventTimeAnimation event_type = EventTimeAnimation.DURATION;

    public float line_duration = 1F;
    //    public bool save_point= false;
    //    public float ball_speed = 100;
    public void Add()
    {
        EventData Event= new EventData();
        Event.time = Game.time;
        setEvent(Event);
        Game.table.addEvent(Event);
    }
    public void setEvent(EventData Event)
    {
        Event.c_pos[0] = camera_position.x;
        Event.c_pos[1] = camera_position.y;
        Event.colorA = new float[] { ColorA.r, ColorA.g, ColorA.b };
        Event.colorB = new float[] { ColorB.r, ColorB.g, ColorB.b };
        Event.c_rot = camera_rotation;
        Event.c_scale = camera_scale;
        Event.duration = duration;
        Event.duration_color = duration_color;
        Event.motion = motion;
        Event.interpole = interpole;
        Event.interpole_color = interpoleColor;
        Event.screen = screen;
        Event.speed = speed;
        Event.showPath = showPath;
        Event.event_type = event_type;
        Event.amplitude = amplitude;
        Event.line_duration = line_duration;
//        Event.save_point = save_point;

    }
    public void GetNoteTime()
    {
        Game.time = NotePosition.path.lower(Game.time).time;
    }
    public void GetCameraPosition()
    {
        camera_position = Camera.main.transform.position;
        camera_scale = Camera.main.orthographicSize;
        camera_rotation = Camera.main.transform.rotation.eulerAngles.z;
    }
    public void setEventNow()
    {
        EventData Event = Game.table.searchEvent(Game.time);
        setEvent(Event);
    }
    public void getDataNow()
    {
        var Event = Game.table.searchEvent(Game.time);
        camera_position.x = Event.c_pos[0];
        camera_position.y= Event.c_pos[1] ;
        ColorA=  new Color(Event.colorA[0], Event.colorA[1], Event.colorA[2]);
        ColorB = new Color(Event.colorB[0], Event.colorB[1], Event.colorB[2]);
        camera_rotation = Event.c_rot;
        camera_scale = Event.c_scale;
        duration = Event.duration;
        duration_color = Event.duration_color;
        motion = Event.motion;
        interpole = Event.interpole;
        interpoleColor = Event.interpole_color;
        screen = Event.screen;
        speed = Event.speed;
        showPath = Event.showPath;
        event_type = Event.event_type;
        amplitude = Event.amplitude;
        line_duration = Event.line_duration;
    }
    public void deleteNow()
    {

        Game.table.deleteEvent(Game.table.searchEvent(Game.time));
    }
    */
}
