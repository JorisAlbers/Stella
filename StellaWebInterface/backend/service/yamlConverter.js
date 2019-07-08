/**
 * /// <summary> Not set. </summary>
 * Unknown,
 *
 * /// <summary> Repeats a pattern over the LedStrip and moves the start point of the pattern each frame by +1. </summary>
 * SlidingPattern,
 *
 * /// <summary> Repeats one or multiple pattern(s) over the length of the LedStrip.
 * /// Each frame is a Color[] pattern repeated over the LedStrip. </summary>
 * RepeatingPattern,
 *
 * /// <summary> Moves a pattern over the led strip from the start of the led strip till the end. </summary>
 * MovingPattern,
 *
 * /// <summary> Duplicates a pattern a random number of times. The patterns create are randomly placed and fade over time</summary>
 * RandomFade,
 *
 * /// <summary> Creates a fading pulse in both directions, starting from a given position </summary>
 * FadingPulse,
 *
 * /// <summary> Scans an image to create an image where x,y = x = pixelIndex, y = frameIndex. </summary>
 * Bitmap,
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

function Bitmap(startIndex, stripLength, frameWaitMs, relativeStart, imageName) {
  this.class = 'Bitmap';
  this.startIndex = startIndex;
  this.stripLength = stripLength;
  this.frameWaitMs = frameWaitMs;
  this.relativeStart = relativeStart;
  this.imageName = imageName
}

const BitmapYamlType = new yaml.Type('!Bitmap', {
  kind: 'mapping',
  construct: function (data) {
    data = data || {}; // in case of empty node
    return new Bitmap(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0, data.ImageName || '');
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
    data = data || {}; // in case of empty node
    return new CachedAnimation(data.StartIndex || 0, data.StripLength || 0, data.FrameWaitMs || 5, data.RelativeStart || 0);
  },
  instanceOf: CachedAnimation
});

const STORYBOARD_SCHEMA = yaml.Schema.create([MovingPatternYamlType, BitmapYamlType, CachedAnimationYamlType, StoryboardYamlType]);

function yamlToJson(yamlString) {
  try {
    return yaml.load(yamlString, {schema: STORYBOARD_SCHEMA});
  } catch (e) {
    console.log(e)
  }
}

exports = module.exports = yamlToJson;