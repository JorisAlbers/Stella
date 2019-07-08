const yaml = require('js-yaml');

function Storyboard(name, animations) {
  if (animations) {
    if (!animations.every(function (animations) {
      return animations instanceof MovingPattern;
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
  // `represent` is omitted here. So, Space objects will be dumped as is.
  // That is regular mapping with three key-value pairs but with !space tag.
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
  // `represent` is omitted here. So, Space objects will be dumped as is.
  // That is regular mapping with three key-value pairs but with !space tag.
});

const STORYBOARD_SCHEMA = yaml.Schema.create([MovingPatternYamlType, StoryboardYamlType]);

function yamlToJson(yamlString) {
  try {
    return yaml.load(yamlString, { schema: STORYBOARD_SCHEMA });
  } catch (e) {
    console.log(e)
  }
}

exports = module.exports = yamlToJson;