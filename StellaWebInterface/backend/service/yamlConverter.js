/**
 * /// <summary> Not set. </summary>
 * Unknown,
 *
 * /// <summary> Repeats a pattern over the LedStrip and moves the start point of the pattern each frame by +1. </summary>
 * SlidingPattern,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    Pattern       Array[Object]
 *
 * /// <summary> Repeats one or multiple pattern(s) over the length of the LedStrip.
 * /// Each frame is a Color[] pattern repeated over the LedStrip. </summary>
 * RepeatingPattern,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    Pattern       Array[Object]
 *
 * /// <summary> Moves a pattern over the led strip from the start of the led strip till the end. </summary>
 * MovingPattern,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    Pattern       Array[Object]
 *
 * /// <summary> Duplicates a pattern a random number of times. The patterns create are randomly placed and fade over time</summary>
 * RandomFade,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    FadeSteps     Number
 *    Pattern       Array[Object]
 *
 * /// <summary> Creates a fading pulse in both directions, starting from a given position </summary>
 * FadingPulse,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    FadeSteps     Number
 *    Color         Object //todo define this object
 *
 * /// <summary> Scans an image to create an image where x,y = x = pixelIndex, y = frameIndex. </summary>
 * Bitmap,
 *    StartIndex    Number
 *    StripLength   Number
 *    FrameWaitMs   Number
 *    RelativeStart Number
 *    ImageName     String
 *    Wraps         Boolean
 */


const yaml = require('js-yaml');

function Storyboard(name, animations) {
  if (animations) {
    if (!animations.every(function (animation) {
      switch (animation.class) {
        case 'MovingPattern':
          return animation instanceof MovingPattern;
        case 'Bitmap':
          return animation instanceof Bitmap;
        case 'CachedAnimation':
          return animation instanceof CachedAnimation;
        case 'RandomFade':
          return animation instanceof RandomFade;
        case 'FadingPulse':
          return animation instanceof FadingPulse;
        case 'RepeatingPattern':
          return animation instanceof RepeatingPattern;
        case 'SlidingPattern':
          return animation instanceof SlidingPattern;
        default:
          return animation instanceof None;
      }
    })) {
      throw new Error('A non-Point inside a points array!');
    }
  }

  this.class = 'Storyboard';
  this.name = name;
  this.animations = animations;
}

const StoryboardYamlType = new yaml.Type('!Storyboard', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new Storyboard(data.Name || 'null', data.Animations || []);
  },
  instanceOf: Storyboard
});

function None() {
  this.class = 'None'
}

const NoneYamlType = new yaml.Type('!None', {
  kind: 'mapping',
  construct: function () {
    return new None();
  },
  instanceOf: None
});


function MovingPattern(startIndex, stripLength, frameWaitMs, relativeStart, pattern) {
  this.class = 'MovingPattern';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.pattern = pattern;
}

const MovingPatternYamlType = new yaml.Type('!MovingPattern', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new MovingPattern(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.Pattern || []);
  },
  instanceOf: MovingPattern
});

function Bitmap(startIndex, stripLength, frameWaitMs, relativeStart, imageName, wraps) {
  this.class = 'Bitmap';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.imageName = imageName;
  this.wraps = wraps
}

const BitmapYamlType = new yaml.Type('!Bitmap', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new Bitmap(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.ImageName || '', data.Wraps || false);
  },
  instanceOf: Bitmap
});

function CachedAnimation(startIndex, stripLength, frameWaitMs, relativeStart) {
  this.class = 'CachedAnimation';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
}

const CachedAnimationYamlType = new yaml.Type('!CachedAnimation', {
  kind: 'mapping',
  construct: function (data) {
    console.log("Deprecation warning! 'CachedAnimation' Is deprecated please use 'Bitmap' instead");
    data = data || {}; // in case of empty node
    return new CachedAnimation(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0);
  },
  instanceOf: CachedAnimation
});

function FadingPulse(startIndex, stripLength, frameWaitMs, relativeStart, fadeSteps, color) {
  this.class = 'FadingPulse';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.fadeSteps = fadeSteps;
  this.color = color;
}

const FadingPulseYamlType = new yaml.Type('!FadingPulse', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    // todo, check what exactly is in data.Color
    console.log('data.Color', data.Color);
    return new FadingPulse(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.FadeSteps || 0, data.Color || [0, 0, 0]);
  },
  instanceOf: FadingPulse
});

function RandomFade(startIndex, stripLength, frameWaitMs, relativeStart, fadeSteps, pattern) {
  this.class = 'RandomFade';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.fadeSteps = fadeSteps;
  this.pattern = pattern;
}

const RandomFadeYamlType = new yaml.Type('!RandomFade', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new RandomFade(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.FadeSteps || 0, data.Pattern || []);
  },
  instanceOf: RandomFade
});

function RepeatingPattern(startIndex, stripLength, frameWaitMs, relativeStart, pattern) {
  this.class = 'RepeatingPattern';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.pattern = pattern;
}

const RepeatingPatternYamlType = new yaml.Type('!RepeatingPattern', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new RepeatingPattern(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.Pattern || []);
  },
  instanceOf: RepeatingPattern
});

function SlidingPattern(startIndex, stripLength, frameWaitMs, relativeStart, pattern) {
  this.class = 'SlidingPattern';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.pattern = pattern;
}

const SlidingPatternYamlType = new yaml.Type('!SlidingPattern', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new SlidingPattern(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.Pattern || []);
  },
  instanceOf: SlidingPattern
});

const STORYBOARD_SCHEMA = yaml.Schema.create([NoneYamlType, MovingPatternYamlType, FadingPulseYamlType, SlidingPatternYamlType, RandomFadeYamlType, RepeatingPatternYamlType, BitmapYamlType, CachedAnimationYamlType, StoryboardYamlType]);

function yamlToJson(yamlString) {
  try {
    return yaml.load(yamlString, {schema: STORYBOARD_SCHEMA});
  } catch (e) {
    console.log(e)
  }
}

exports = module.exports = yamlToJson;