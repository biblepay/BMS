"undefined" != typeof window &&
    (function (t, e) {
        "object" == typeof exports && "object" == typeof module ? (module.exports = e()) : "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? (exports.Hls = e()) : (t.Hls = e());
    })(this, function () {
        return (function (t) {
            var e = {};
            function r(i) {
                if (e[i]) return e[i].exports;
                var n = (e[i] = { i: i, l: !1, exports: {} });
                return t[i].call(n.exports, n, n.exports, r), (n.l = !0), n.exports;
            }
            return (
                (r.m = t),
                (r.c = e),
                (r.d = function (t, e, i) {
                    r.o(t, e) || Object.defineProperty(t, e, { enumerable: !0, get: i });
                }),
                (r.r = function (t) {
                    "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(t, Symbol.toStringTag, { value: "Module" }), Object.defineProperty(t, "__esModule", { value: !0 });
                }),
                (r.t = function (t, e) {
                    if ((1 & e && (t = r(t)), 8 & e)) return t;
                    if (4 & e && "object" == typeof t && t && t.__esModule) return t;
                    var i = Object.create(null);
                    if ((r.r(i), Object.defineProperty(i, "default", { enumerable: !0, value: t }), 2 & e && "string" != typeof t))
                        for (var n in t)
                            r.d(
                                i,
                                n,
                                function (e) {
                                    return t[e];
                                }.bind(null, n)
                            );
                    return i;
                }),
                (r.n = function (t) {
                    var e =
                        t && t.__esModule
                            ? function () {
                                  return t.default;
                              }
                            : function () {
                                  return t;
                              };
                    return r.d(e, "a", e), e;
                }),
                (r.o = function (t, e) {
                    return Object.prototype.hasOwnProperty.call(t, e);
                }),
                (r.p = "/dist/"),
                r((r.s = 20))
            );
        })([
            function (t, e, r) {
                "use strict";
                var i;
                r.d(e, "a", function () {
                    return i;
                }),
                    (function (t) {
                        (t.MEDIA_ATTACHING = "hlsMediaAttaching"),
                            (t.MEDIA_ATTACHED = "hlsMediaAttached"),
                            (t.MEDIA_DETACHING = "hlsMediaDetaching"),
                            (t.MEDIA_DETACHED = "hlsMediaDetached"),
                            (t.BUFFER_RESET = "hlsBufferReset"),
                            (t.BUFFER_CODECS = "hlsBufferCodecs"),
                            (t.BUFFER_CREATED = "hlsBufferCreated"),
                            (t.BUFFER_APPENDING = "hlsBufferAppending"),
                            (t.BUFFER_APPENDED = "hlsBufferAppended"),
                            (t.BUFFER_EOS = "hlsBufferEos"),
                            (t.BUFFER_FLUSHING = "hlsBufferFlushing"),
                            (t.BUFFER_FLUSHED = "hlsBufferFlushed"),
                            (t.MANIFEST_LOADING = "hlsManifestLoading"),
                            (t.MANIFEST_LOADED = "hlsManifestLoaded"),
                            (t.MANIFEST_PARSED = "hlsManifestParsed"),
                            (t.LEVEL_SWITCHING = "hlsLevelSwitching"),
                            (t.LEVEL_SWITCHED = "hlsLevelSwitched"),
                            (t.LEVEL_LOADING = "hlsLevelLoading"),
                            (t.LEVEL_LOADED = "hlsLevelLoaded"),
                            (t.LEVEL_UPDATED = "hlsLevelUpdated"),
                            (t.LEVEL_PTS_UPDATED = "hlsLevelPtsUpdated"),
                            (t.LEVELS_UPDATED = "hlsLevelsUpdated"),
                            (t.AUDIO_TRACKS_UPDATED = "hlsAudioTracksUpdated"),
                            (t.AUDIO_TRACK_SWITCHING = "hlsAudioTrackSwitching"),
                            (t.AUDIO_TRACK_SWITCHED = "hlsAudioTrackSwitched"),
                            (t.AUDIO_TRACK_LOADING = "hlsAudioTrackLoading"),
                            (t.AUDIO_TRACK_LOADED = "hlsAudioTrackLoaded"),
                            (t.SUBTITLE_TRACKS_UPDATED = "hlsSubtitleTracksUpdated"),
                            (t.SUBTITLE_TRACKS_CLEARED = "hlsSubtitleTracksCleared"),
                            (t.SUBTITLE_TRACK_SWITCH = "hlsSubtitleTrackSwitch"),
                            (t.SUBTITLE_TRACK_LOADING = "hlsSubtitleTrackLoading"),
                            (t.SUBTITLE_TRACK_LOADED = "hlsSubtitleTrackLoaded"),
                            (t.SUBTITLE_FRAG_PROCESSED = "hlsSubtitleFragProcessed"),
                            (t.CUES_PARSED = "hlsCuesParsed"),
                            (t.NON_NATIVE_TEXT_TRACKS_FOUND = "hlsNonNativeTextTracksFound"),
                            (t.INIT_PTS_FOUND = "hlsInitPtsFound"),
                            (t.FRAG_LOADING = "hlsFragLoading"),
                            (t.FRAG_LOAD_EMERGENCY_ABORTED = "hlsFragLoadEmergencyAborted"),
                            (t.FRAG_LOADED = "hlsFragLoaded"),
                            (t.FRAG_DECRYPTED = "hlsFragDecrypted"),
                            (t.FRAG_PARSING_INIT_SEGMENT = "hlsFragParsingInitSegment"),
                            (t.FRAG_PARSING_USERDATA = "hlsFragParsingUserdata"),
                            (t.FRAG_PARSING_METADATA = "hlsFragParsingMetadata"),
                            (t.FRAG_PARSED = "hlsFragParsed"),
                            (t.FRAG_BUFFERED = "hlsFragBuffered"),
                            (t.FRAG_CHANGED = "hlsFragChanged"),
                            (t.FPS_DROP = "hlsFpsDrop"),
                            (t.FPS_DROP_LEVEL_CAPPING = "hlsFpsDropLevelCapping"),
                            (t.ERROR = "hlsError"),
                            (t.DESTROYING = "hlsDestroying"),
                            (t.KEY_LOADING = "hlsKeyLoading"),
                            (t.KEY_LOADED = "hlsKeyLoaded"),
                            (t.LIVE_BACK_BUFFER_REACHED = "hlsLiveBackBufferReached"),
                            (t.BACK_BUFFER_REACHED = "hlsBackBufferReached");
                    })(i || (i = {}));
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return o;
                }),
                    r.d(e, "b", function () {
                        return l;
                    });
                var i = function () {},
                    n = { trace: i, debug: i, log: i, warn: i, info: i, error: i },
                    a = n;
                function s(t) {
                    var e = self.console[t];
                    return e ? e.bind(self.console, "[" + t + "] >") : i;
                }
                function o(t) {
                    if ((self.console && !0 === t) || "object" == typeof t) {
                        !(function (t) {
                            for (var e = arguments.length, r = new Array(e > 1 ? e - 1 : 0), i = 1; i < e; i++) r[i - 1] = arguments[i];
                            r.forEach(function (e) {
                                a[e] = t[e] ? t[e].bind(t) : s(e);
                            });
                        })(t, "debug", "log", "info", "warn", "error");
                        try {
                            a.log();
                        } catch (t) {
                            a = n;
                        }
                    } else a = n;
                }
                var l = n;
            },
            function (t, e, r) {
                "use strict";
                var i, n;
                r.d(e, "b", function () {
                    return i;
                }),
                    r.d(e, "a", function () {
                        return n;
                    }),
                    (function (t) {
                        (t.NETWORK_ERROR = "networkError"), (t.MEDIA_ERROR = "mediaError"), (t.KEY_SYSTEM_ERROR = "keySystemError"), (t.MUX_ERROR = "muxError"), (t.OTHER_ERROR = "otherError");
                    })(i || (i = {})),
                    (function (t) {
                        (t.KEY_SYSTEM_NO_KEYS = "keySystemNoKeys"),
                            (t.KEY_SYSTEM_NO_ACCESS = "keySystemNoAccess"),
                            (t.KEY_SYSTEM_NO_SESSION = "keySystemNoSession"),
                            (t.KEY_SYSTEM_LICENSE_REQUEST_FAILED = "keySystemLicenseRequestFailed"),
                            (t.KEY_SYSTEM_NO_INIT_DATA = "keySystemNoInitData"),
                            (t.MANIFEST_LOAD_ERROR = "manifestLoadError"),
                            (t.MANIFEST_LOAD_TIMEOUT = "manifestLoadTimeOut"),
                            (t.MANIFEST_PARSING_ERROR = "manifestParsingError"),
                            (t.MANIFEST_INCOMPATIBLE_CODECS_ERROR = "manifestIncompatibleCodecsError"),
                            (t.LEVEL_EMPTY_ERROR = "levelEmptyError"),
                            (t.LEVEL_LOAD_ERROR = "levelLoadError"),
                            (t.LEVEL_LOAD_TIMEOUT = "levelLoadTimeOut"),
                            (t.LEVEL_SWITCH_ERROR = "levelSwitchError"),
                            (t.AUDIO_TRACK_LOAD_ERROR = "audioTrackLoadError"),
                            (t.AUDIO_TRACK_LOAD_TIMEOUT = "audioTrackLoadTimeOut"),
                            (t.SUBTITLE_LOAD_ERROR = "subtitleTrackLoadError"),
                            (t.SUBTITLE_TRACK_LOAD_TIMEOUT = "subtitleTrackLoadTimeOut"),
                            (t.FRAG_LOAD_ERROR = "fragLoadError"),
                            (t.FRAG_LOAD_TIMEOUT = "fragLoadTimeOut"),
                            (t.FRAG_DECRYPT_ERROR = "fragDecryptError"),
                            (t.FRAG_PARSING_ERROR = "fragParsingError"),
                            (t.REMUX_ALLOC_ERROR = "remuxAllocError"),
                            (t.KEY_LOAD_ERROR = "keyLoadError"),
                            (t.KEY_LOAD_TIMEOUT = "keyLoadTimeOut"),
                            (t.BUFFER_ADD_CODEC_ERROR = "bufferAddCodecError"),
                            (t.BUFFER_INCOMPATIBLE_CODECS_ERROR = "bufferIncompatibleCodecsError"),
                            (t.BUFFER_APPEND_ERROR = "bufferAppendError"),
                            (t.BUFFER_APPENDING_ERROR = "bufferAppendingError"),
                            (t.BUFFER_STALLED_ERROR = "bufferStalledError"),
                            (t.BUFFER_FULL_ERROR = "bufferFullError"),
                            (t.BUFFER_SEEK_OVER_HOLE = "bufferSeekOverHole"),
                            (t.BUFFER_NUDGE_ON_STALL = "bufferNudgeOnStall"),
                            (t.INTERNAL_EXCEPTION = "internalException"),
                            (t.INTERNAL_ABORTED = "aborted"),
                            (t.UNKNOWN = "unknown");
                    })(n || (n = {}));
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return i;
                });
                var i =
                    Number.isFinite ||
                    function (t) {
                        return "number" == typeof t && isFinite(t);
                    };
                Number.MAX_SAFE_INTEGER;
            },
            function (t, e, r) {
                "use strict";
                var i, n;
                r.d(e, "a", function () {
                    return i;
                }),
                    r.d(e, "b", function () {
                        return n;
                    }),
                    (function (t) {
                        (t.MANIFEST = "manifest"), (t.LEVEL = "level"), (t.AUDIO_TRACK = "audioTrack"), (t.SUBTITLE_TRACK = "subtitleTrack");
                    })(i || (i = {})),
                    (function (t) {
                        (t.MAIN = "main"), (t.AUDIO = "audio"), (t.SUBTITLE = "subtitle");
                    })(n || (n = {}));
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return i;
                }),
                    r.d(e, "b", function () {
                        return g;
                    }),
                    r.d(e, "c", function () {
                        return v;
                    });
                var i,
                    n = r(3),
                    a = r(11),
                    s = r(1),
                    o = r(17),
                    l = r(12);
                function u(t, e) {
                    (t.prototype = Object.create(e.prototype)), (t.prototype.constructor = t), h(t, e);
                }
                function h(t, e) {
                    return (h =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                function d(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function c(t, e, r) {
                    return e && d(t.prototype, e), r && d(t, r), t;
                }
                !(function (t) {
                    (t.AUDIO = "audio"), (t.VIDEO = "video"), (t.AUDIOVIDEO = "audiovideo");
                })(i || (i = {}));
                var f = (function () {
                        function t(t) {
                            var e;
                            (this._byteRange = null),
                                (this._url = null),
                                (this.baseurl = void 0),
                                (this.relurl = void 0),
                                (this.elementaryStreams = (((e = {})[i.AUDIO] = null), (e[i.VIDEO] = null), (e[i.AUDIOVIDEO] = null), e)),
                                (this.baseurl = t);
                        }
                        return (
                            (t.prototype.setByteRange = function (t, e) {
                                var r = t.split("@", 2),
                                    i = [];
                                1 === r.length ? (i[0] = e ? e.byteRangeEndOffset : 0) : (i[0] = parseInt(r[1])), (i[1] = parseInt(r[0]) + i[0]), (this._byteRange = i);
                            }),
                            c(t, [
                                {
                                    key: "byteRange",
                                    get: function () {
                                        return this._byteRange ? this._byteRange : [];
                                    },
                                },
                                {
                                    key: "byteRangeStartOffset",
                                    get: function () {
                                        return this.byteRange[0];
                                    },
                                },
                                {
                                    key: "byteRangeEndOffset",
                                    get: function () {
                                        return this.byteRange[1];
                                    },
                                },
                                {
                                    key: "url",
                                    get: function () {
                                        return !this._url && this.baseurl && this.relurl && (this._url = Object(a.buildAbsoluteURL)(this.baseurl, this.relurl, { alwaysNormalize: !0 })), this._url || "";
                                    },
                                    set: function (t) {
                                        this._url = t;
                                    },
                                },
                            ]),
                            t
                        );
                    })(),
                    g = (function (t) {
                        function e(e, r) {
                            var i;
                            return (
                                ((i = t.call(this, r) || this)._decryptdata = null),
                                (i.rawProgramDateTime = null),
                                (i.programDateTime = null),
                                (i.tagList = []),
                                (i.duration = 0),
                                (i.sn = 0),
                                (i.levelkey = void 0),
                                (i.type = void 0),
                                (i.loader = null),
                                (i.level = -1),
                                (i.cc = 0),
                                (i.startPTS = void 0),
                                (i.endPTS = void 0),
                                (i.appendedPTS = void 0),
                                (i.startDTS = void 0),
                                (i.endDTS = void 0),
                                (i.start = 0),
                                (i.deltaPTS = void 0),
                                (i.maxStartPTS = void 0),
                                (i.minEndPTS = void 0),
                                (i.stats = new l.a()),
                                (i.urlId = 0),
                                (i.data = void 0),
                                (i.bitrateTest = !1),
                                (i.title = null),
                                (i.initSegment = null),
                                (i.type = e),
                                i
                            );
                        }
                        u(e, t);
                        var r = e.prototype;
                        return (
                            (r.createInitializationVector = function (t) {
                                for (var e = new Uint8Array(16), r = 12; r < 16; r++) e[r] = (t >> (8 * (15 - r))) & 255;
                                return e;
                            }),
                            (r.setDecryptDataFromLevelKey = function (t, e) {
                                var r = t;
                                return "AES-128" === (null == t ? void 0 : t.method) && t.uri && !t.iv && (((r = o.a.fromURI(t.uri)).method = t.method), (r.iv = this.createInitializationVector(e)), (r.keyFormat = "identity")), r;
                            }),
                            (r.setElementaryStreamInfo = function (t, e, r, i, n, a) {
                                void 0 === a && (a = !1);
                                var s = this.elementaryStreams,
                                    o = s[t];
                                o
                                    ? ((o.startPTS = Math.min(o.startPTS, e)), (o.endPTS = Math.max(o.endPTS, r)), (o.startDTS = Math.min(o.startDTS, i)), (o.endDTS = Math.max(o.endDTS, n)))
                                    : (s[t] = { startPTS: e, endPTS: r, startDTS: i, endDTS: n, partial: a });
                            }),
                            (r.clearElementaryStreamInfo = function () {
                                var t = this.elementaryStreams;
                                (t[i.AUDIO] = null), (t[i.VIDEO] = null), (t[i.AUDIOVIDEO] = null);
                            }),
                            c(e, [
                                {
                                    key: "decryptdata",
                                    get: function () {
                                        if (!this.levelkey && !this._decryptdata) return null;
                                        if (!this._decryptdata && this.levelkey) {
                                            var t = this.sn;
                                            "number" != typeof t &&
                                                (this.levelkey && "AES-128" === this.levelkey.method && !this.levelkey.iv && s.b.warn('missing IV for initialization segment with method="' + this.levelkey.method + '" - compliance issue'),
                                                (t = 0)),
                                                (this._decryptdata = this.setDecryptDataFromLevelKey(this.levelkey, t));
                                        }
                                        return this._decryptdata;
                                    },
                                },
                                {
                                    key: "end",
                                    get: function () {
                                        return this.start + this.duration;
                                    },
                                },
                                {
                                    key: "endProgramDateTime",
                                    get: function () {
                                        if (null === this.programDateTime) return null;
                                        if (!Object(n.a)(this.programDateTime)) return null;
                                        var t = Object(n.a)(this.duration) ? this.duration : 0;
                                        return this.programDateTime + 1e3 * t;
                                    },
                                },
                                {
                                    key: "encrypted",
                                    get: function () {
                                        var t;
                                        return !(null === (t = this.decryptdata) || void 0 === t || !t.keyFormat || !this.decryptdata.uri);
                                    },
                                },
                            ]),
                            e
                        );
                    })(f),
                    v = (function (t) {
                        function e(e, r, i, n, a) {
                            var s;
                            ((s = t.call(this, i) || this).fragOffset = 0),
                                (s.duration = 0),
                                (s.gap = !1),
                                (s.independent = !1),
                                (s.relurl = void 0),
                                (s.fragment = void 0),
                                (s.index = void 0),
                                (s.stats = new l.a()),
                                (s.duration = e.decimalFloatingPoint("DURATION")),
                                (s.gap = e.bool("GAP")),
                                (s.independent = e.bool("INDEPENDENT")),
                                (s.relurl = e.enumeratedString("URI")),
                                (s.fragment = r),
                                (s.index = n);
                            var o = e.enumeratedString("BYTERANGE");
                            return o && s.setByteRange(o, a), a && (s.fragOffset = a.fragOffset + a.duration), s;
                        }
                        return (
                            u(e, t),
                            c(e, [
                                {
                                    key: "start",
                                    get: function () {
                                        return this.fragment.start + this.fragOffset;
                                    },
                                },
                                {
                                    key: "end",
                                    get: function () {
                                        return this.start + this.duration;
                                    },
                                },
                                {
                                    key: "loaded",
                                    get: function () {
                                        var t = this.elementaryStreams;
                                        return !!(t.audio || t.video || t.audiovideo);
                                    },
                                },
                            ]),
                            e
                        );
                    })(f);
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "b", function () {
                    return h;
                }),
                    r.d(e, "g", function () {
                        return d;
                    }),
                    r.d(e, "f", function () {
                        return c;
                    }),
                    r.d(e, "d", function () {
                        return f;
                    }),
                    r.d(e, "c", function () {
                        return g;
                    }),
                    r.d(e, "e", function () {
                        return p;
                    }),
                    r.d(e, "h", function () {
                        return m;
                    }),
                    r.d(e, "a", function () {
                        return y;
                    });
                var i = r(9),
                    n = r(5),
                    a = Math.pow(2, 32) - 1,
                    s = [].push;
                function o(t) {
                    return String.fromCharCode.apply(null, t);
                }
                function l(t, e) {
                    "data" in t && ((e += t.start), (t = t.data));
                    var r = (t[e] << 24) | (t[e + 1] << 16) | (t[e + 2] << 8) | t[e + 3];
                    return r < 0 ? 4294967296 + r : r;
                }
                function u(t, e, r) {
                    "data" in t && ((e += t.start), (t = t.data)), (t[e] = r >> 24), (t[e + 1] = (r >> 16) & 255), (t[e + 2] = (r >> 8) & 255), (t[e + 3] = 255 & r);
                }
                function h(t, e) {
                    var r,
                        i,
                        n,
                        a = [];
                    if (!e.length) return a;
                    "data" in t ? ((r = t.data), (i = t.start), (n = t.end)) : ((i = 0), (n = (r = t).byteLength));
                    for (var u = i; u < n; ) {
                        var d = l(r, u),
                            c = d > 1 ? u + d : n;
                        if (o(r.subarray(u + 4, u + 8)) === e[0])
                            if (1 === e.length) a.push({ data: r, start: u + 8, end: c });
                            else {
                                var f = h({ data: r, start: u + 8, end: c }, e.slice(1));
                                f.length && s.apply(a, f);
                            }
                        u = c;
                    }
                    return a;
                }
                function d(t) {
                    var e = h(t, ["moov"])[0],
                        r = e ? e.end : null,
                        i = h(t, ["sidx"]);
                    if (!i || !i[0]) return null;
                    var n = [],
                        a = i[0],
                        s = a.data[0],
                        o = 0 === s ? 8 : 16,
                        u = l(a, o);
                    o += 4;
                    (o += 0 === s ? 8 : 16), (o += 2);
                    var d = a.end + 0,
                        c = (function (t, e) {
                            "data" in t && ((e += t.start), (t = t.data));
                            var r = (t[e] << 8) | t[e + 1];
                            return r < 0 ? 65536 + r : r;
                        })(a, o);
                    o += 2;
                    for (var f = 0; f < c; f++) {
                        var g = o,
                            v = l(a, g);
                        g += 4;
                        var p = 2147483647 & v;
                        if (1 === (2147483648 & v) >>> 31) return console.warn("SIDX has hierarchical references (not supported)"), null;
                        var m = l(a, g);
                        (g += 4), n.push({ referenceSize: p, subsegmentDuration: m, info: { duration: m / u, start: d, end: d + p - 1 } }), (d += p), (o = g += 4);
                    }
                    return { earliestPresentationTime: 0, timescale: u, version: s, referencesCount: c, references: n, moovEndOffset: r };
                }
                function c(t) {
                    for (var e = [], r = h(t, ["moov", "trak"]), i = 0; i < r.length; i++) {
                        var a = r[i],
                            s = h(a, ["tkhd"])[0];
                        if (s) {
                            var u = s.data[s.start],
                                d = 0 === u ? 12 : 20,
                                c = l(s, d),
                                f = h(a, ["mdia", "mdhd"])[0];
                            if (f) {
                                var g = l(f, (d = 0 === (u = f.data[f.start]) ? 12 : 20)),
                                    v = h(a, ["mdia", "hdlr"])[0];
                                if (v) {
                                    var p = o(v.data.subarray(v.start + 8, v.start + 12)),
                                        m = { soun: n.a.AUDIO, vide: n.a.VIDEO }[p];
                                    if (m) {
                                        var y = h(a, ["mdia", "minf", "stbl", "stsd"])[0],
                                            T = void 0;
                                        y && (T = o(y.data.subarray(y.start + 12, y.start + 16))), (e[c] = { timescale: g, type: m }), (e[m] = { timescale: g, id: c, codec: T });
                                    }
                                }
                            }
                        }
                    }
                    return (
                        h(t, ["moov", "mvex", "trex"]).forEach(function (t) {
                            var r = l(t, 4),
                                i = e[r];
                            i && (i.default = { duration: l(t, 12), flags: l(t, 20) });
                        }),
                        e
                    );
                }
                function f(t, e) {
                    return (
                        h(e, ["moof", "traf"]).reduce(function (e, r) {
                            var i = h(r, ["tfdt"])[0],
                                n = i.data[i.start],
                                a = h(r, ["tfhd"]).reduce(function (e, r) {
                                    var a = l(r, 4),
                                        s = t[a];
                                    if (s) {
                                        var o = l(i, 4);
                                        1 === n && ((o *= Math.pow(2, 32)), (o += l(i, 8)));
                                        var u = o / (s.timescale || 9e4);
                                        if (isFinite(u) && (null === e || u < e)) return u;
                                    }
                                    return e;
                                }, null);
                            return null !== a && isFinite(a) && (null === e || a < e) ? a : e;
                        }, null) || 0
                    );
                }
                function g(t, e) {
                    for (var r = 0, i = 0, a = 0, s = h(t, ["moof", "traf"]), o = 0; o < s.length; o++) {
                        var u = s[o],
                            c = h(u, ["tfhd"])[0],
                            f = e[l(c, 4)];
                        if (f) {
                            var g = f.default,
                                p = l(c, 0) | (null == g ? void 0 : g.flags),
                                m = null == g ? void 0 : g.duration;
                            8 & p && (m = l(c, 2 & p ? 12 : 8));
                            for (var y = f.timescale || 9e4, T = h(u, ["trun"]), b = 0; b < T.length; b++) {
                                if (m) r = m * l(T[b], 4);
                                else r = v(T[b]);
                                f.type === n.a.VIDEO ? (i += r / y) : f.type === n.a.AUDIO && (a += r / y);
                            }
                        }
                    }
                    if (0 === i && 0 === a) {
                        var E = d(t);
                        if (null != E && E.references)
                            return E.references.reduce(function (t, e) {
                                return t + e.info.duration || 0;
                            }, 0);
                    }
                    return i || a;
                }
                function v(t) {
                    var e = l(t, 0),
                        r = 8;
                    1 & e && (r += 4), 4 & e && (r += 4);
                    for (var i = 0, n = l(t, 4), a = 0; a < n; a++) {
                        if (256 & e) (i += l(t, r)), (r += 4);
                        512 & e && (r += 4), 1024 & e && (r += 4), 2048 & e && (r += 4);
                    }
                    return i;
                }
                function p(t, e, r) {
                    h(e, ["moof", "traf"]).forEach(function (e) {
                        h(e, ["tfhd"]).forEach(function (i) {
                            var n = l(i, 4),
                                s = t[n];
                            if (s) {
                                var o = s.timescale || 9e4;
                                h(e, ["tfdt"]).forEach(function (t) {
                                    var e = t.data[t.start],
                                        i = l(t, 4);
                                    if (0 === e) u(t, 4, i - r * o);
                                    else {
                                        (i *= Math.pow(2, 32)), (i += l(t, 8)), (i -= r * o), (i = Math.max(i, 0));
                                        var n = Math.floor(i / (a + 1)),
                                            s = Math.floor(i % (a + 1));
                                        u(t, 4, n), u(t, 8, s);
                                    }
                                });
                            }
                        });
                    });
                }
                function m(t) {
                    var e = { valid: null, remainder: null },
                        r = h(t, ["moof"]);
                    if (!r) return e;
                    if (r.length < 2) return (e.remainder = t), e;
                    var n = r[r.length - 1];
                    return (e.valid = Object(i.a)(t, 0, n.start - 8)), (e.remainder = Object(i.a)(t, n.start - 8)), e;
                }
                function y(t, e) {
                    var r = new Uint8Array(t.length + e.length);
                    return r.set(t), r.set(e, t.length), r;
                }
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "b", function () {
                    return s;
                }),
                    r.d(e, "a", function () {
                        return l;
                    }),
                    r.d(e, "d", function () {
                        return u;
                    }),
                    r.d(e, "e", function () {
                        return h;
                    }),
                    r.d(e, "c", function () {
                        return c;
                    }),
                    r.d(e, "f", function () {
                        return y;
                    });
                var i,
                    n = function (t, e) {
                        return e + 10 <= t.length && 73 === t[e] && 68 === t[e + 1] && 51 === t[e + 2] && t[e + 3] < 255 && t[e + 4] < 255 && t[e + 6] < 128 && t[e + 7] < 128 && t[e + 8] < 128 && t[e + 9] < 128;
                    },
                    a = function (t, e) {
                        return e + 10 <= t.length && 51 === t[e] && 68 === t[e + 1] && 73 === t[e + 2] && t[e + 3] < 255 && t[e + 4] < 255 && t[e + 6] < 128 && t[e + 7] < 128 && t[e + 8] < 128 && t[e + 9] < 128;
                    },
                    s = function (t, e) {
                        for (var r = e, i = 0; n(t, e); ) {
                            (i += 10), (i += o(t, e + 6)), a(t, e + 10) && (i += 10), (e += i);
                        }
                        if (i > 0) return t.subarray(r, r + i);
                    },
                    o = function (t, e) {
                        var r = 0;
                        return (r = (127 & t[e]) << 21), (r |= (127 & t[e + 1]) << 14), (r |= (127 & t[e + 2]) << 7), (r |= 127 & t[e + 3]);
                    },
                    l = function (t, e) {
                        return n(t, e) && o(t, e + 6) + 10 <= t.length - e;
                    },
                    u = function (t) {
                        for (var e = c(t), r = 0; r < e.length; r++) {
                            var i = e[r];
                            if (h(i)) return m(i);
                        }
                    },
                    h = function (t) {
                        return t && "PRIV" === t.key && "com.apple.streaming.transportStreamTimestamp" === t.info;
                    },
                    d = function (t) {
                        var e = String.fromCharCode(t[0], t[1], t[2], t[3]),
                            r = o(t, 4);
                        return { type: e, size: r, data: t.subarray(10, 10 + r) };
                    },
                    c = function (t) {
                        for (var e = 0, r = []; n(t, e); ) {
                            for (var i = o(t, e + 6), s = (e += 10) + i; e + 8 < s; ) {
                                var l = d(t.subarray(e)),
                                    u = f(l);
                                u && r.push(u), (e += l.size + 10);
                            }
                            a(t, e) && (e += 10);
                        }
                        return r;
                    },
                    f = function (t) {
                        return "PRIV" === t.type ? g(t) : "W" === t.type[0] ? p(t) : v(t);
                    },
                    g = function (t) {
                        if (!(t.size < 2)) {
                            var e = y(t.data, !0),
                                r = new Uint8Array(t.data.subarray(e.length + 1));
                            return { key: t.type, info: e, data: r.buffer };
                        }
                    },
                    v = function (t) {
                        if (!(t.size < 2)) {
                            if ("TXXX" === t.type) {
                                var e = 1,
                                    r = y(t.data.subarray(e), !0);
                                e += r.length + 1;
                                var i = y(t.data.subarray(e));
                                return { key: t.type, info: r, data: i };
                            }
                            var n = y(t.data.subarray(1));
                            return { key: t.type, data: n };
                        }
                    },
                    p = function (t) {
                        if ("WXXX" === t.type) {
                            if (t.size < 2) return;
                            var e = 1,
                                r = y(t.data.subarray(e), !0);
                            e += r.length + 1;
                            var i = y(t.data.subarray(e));
                            return { key: t.type, info: r, data: i };
                        }
                        var n = y(t.data);
                        return { key: t.type, data: n };
                    },
                    m = function (t) {
                        if (8 === t.data.byteLength) {
                            var e = new Uint8Array(t.data),
                                r = 1 & e[3],
                                i = (e[4] << 23) + (e[5] << 15) + (e[6] << 7) + e[7];
                            return (i /= 45), r && (i += 47721858.84), Math.round(i);
                        }
                    },
                    y = function (t, e) {
                        void 0 === e && (e = !1);
                        var r = T();
                        if (r) {
                            var i = r.decode(t);
                            if (e) {
                                var n = i.indexOf("\0");
                                return -1 !== n ? i.substring(0, n) : i;
                            }
                            return i.replace(/\0/g, "");
                        }
                        for (var a, s, o, l = t.length, u = "", h = 0; h < l; ) {
                            if (0 === (a = t[h++]) && e) return u;
                            if (0 !== a && 3 !== a)
                                switch (a >> 4) {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                        u += String.fromCharCode(a);
                                        break;
                                    case 12:
                                    case 13:
                                        (s = t[h++]), (u += String.fromCharCode(((31 & a) << 6) | (63 & s)));
                                        break;
                                    case 14:
                                        (s = t[h++]), (o = t[h++]), (u += String.fromCharCode(((15 & a) << 12) | ((63 & s) << 6) | ((63 & o) << 0)));
                                }
                        }
                        return u;
                    };
                function T() {
                    return i || void 0 === self.TextDecoder || (i = new self.TextDecoder("utf-8")), i;
                }
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "c", function () {
                    return n;
                }),
                    r.d(e, "b", function () {
                        return a;
                    }),
                    r.d(e, "a", function () {
                        return s;
                    });
                function i(t, e, r, i) {
                    void 0 === r && (r = 1), void 0 === i && (i = !1);
                    var n = t * e * r;
                    return i ? Math.round(n) : n;
                }
                function n(t, e, r, n) {
                    return void 0 === r && (r = 1), void 0 === n && (n = !1), i(t, e, 1 / r, n);
                }
                function a(t, e) {
                    return void 0 === e && (e = !1), i(t, 1e3, 1 / 9e4, e);
                }
                function s(t, e) {
                    return void 0 === e && (e = 1), i(t, 9e4, 1 / e);
                }
            },
            function (t, e, r) {
                "use strict";
                function i(t, e, r) {
                    return Uint8Array.prototype.slice ? t.slice(e, r) : new Uint8Array(Array.prototype.slice.call(t, e, r));
                }
                r.d(e, "a", function () {
                    return i;
                });
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "c", function () {
                    return lt;
                }),
                    r.d(e, "d", function () {
                        return ht;
                    }),
                    r.d(e, "a", function () {
                        return dt;
                    }),
                    r.d(e, "b", function () {
                        return ct;
                    });
                var i = r(0),
                    n = r(2),
                    a = r(15),
                    s = r(3),
                    o = r(7);
                var l = r(6),
                    u = r(9),
                    h = (function () {
                        function t() {
                            (this._audioTrack = void 0), (this._id3Track = void 0), (this.frameIndex = 0), (this.cachedData = null), (this.initPTS = null);
                        }
                        var e = t.prototype;
                        return (
                            (e.resetInitSegment = function (t, e, r) {
                                this._id3Track = { type: "id3", id: 0, pid: -1, inputTimeScale: 9e4, sequenceNumber: 0, samples: [], dropped: 0 };
                            }),
                            (e.resetTimeStamp = function () {}),
                            (e.resetContiguity = function () {}),
                            (e.canParse = function (t, e) {
                                return !1;
                            }),
                            (e.appendFrame = function (t, e, r) {}),
                            (e.demux = function (t, e) {
                                this.cachedData && ((t = Object(l.a)(this.cachedData, t)), (this.cachedData = null));
                                var r,
                                    i,
                                    n = o.b(t, 0),
                                    a = n ? n.length : 0,
                                    s = this._audioTrack,
                                    h = this._id3Track,
                                    c = n ? o.d(n) : void 0,
                                    f = t.length;
                                for ((0 !== this.frameIndex && null !== this.initPTS) || (this.initPTS = d(c, e)), n && n.length > 0 && h.samples.push({ pts: this.initPTS, dts: this.initPTS, data: n }), i = this.initPTS; a < f; ) {
                                    if (this.canParse(t, a)) {
                                        var g = this.appendFrame(s, t, a);
                                        g ? (this.frameIndex++, (i = g.sample.pts), (r = a += g.length)) : (a = f);
                                    } else o.a(t, a) ? ((n = o.b(t, a)), h.samples.push({ pts: i, dts: i, data: n }), (r = a += n.length)) : a++;
                                    if (a === f && r !== f) {
                                        var v = Object(u.a)(t, r);
                                        this.cachedData ? (this.cachedData = Object(l.a)(this.cachedData, v)) : (this.cachedData = v);
                                    }
                                }
                                return {
                                    audioTrack: s,
                                    avcTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                    id3Track: h,
                                    textTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                };
                            }),
                            (e.demuxSampleAes = function (t, e, r) {
                                return Promise.reject(new Error("[" + this + "] This demuxer does not support Sample-AES decryption"));
                            }),
                            (e.flush = function (t) {
                                var e = this.cachedData;
                                return (
                                    e && ((this.cachedData = null), this.demux(e, 0)),
                                    (this.frameIndex = 0),
                                    {
                                        audioTrack: this._audioTrack,
                                        avcTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                        id3Track: this._id3Track,
                                        textTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                    }
                                );
                            }),
                            (e.destroy = function () {}),
                            t
                        );
                    })(),
                    d = function (t, e) {
                        return Object(s.a)(t) ? 90 * t : 9e4 * e;
                    },
                    c = h,
                    f = r(1);
                function g(t, e) {
                    return 255 === t[e] && 240 == (246 & t[e + 1]);
                }
                function v(t, e) {
                    return 1 & t[e + 1] ? 7 : 9;
                }
                function p(t, e) {
                    return ((3 & t[e + 3]) << 11) | (t[e + 4] << 3) | ((224 & t[e + 5]) >>> 5);
                }
                function m(t, e) {
                    return e + 1 < t.length && g(t, e);
                }
                function y(t, e) {
                    if (m(t, e)) {
                        var r = v(t, e);
                        if (e + r >= t.length) return !1;
                        var i = p(t, e);
                        if (i <= r) return !1;
                        var n = e + i;
                        return n === t.length || m(t, n);
                    }
                    return !1;
                }
                function T(t, e, r, a, s) {
                    if (!t.samplerate) {
                        var o = (function (t, e, r, a) {
                            var s,
                                o,
                                l,
                                u,
                                h = navigator.userAgent.toLowerCase(),
                                d = a,
                                c = [96e3, 88200, 64e3, 48e3, 44100, 32e3, 24e3, 22050, 16e3, 12e3, 11025, 8e3, 7350];
                            s = 1 + ((192 & e[r + 2]) >>> 6);
                            var g = (60 & e[r + 2]) >>> 2;
                            if (!(g > c.length - 1))
                                return (
                                    (l = (1 & e[r + 2]) << 2),
                                    (l |= (192 & e[r + 3]) >>> 6),
                                    f.b.log("manifest codec:" + a + ", ADTS type:" + s + ", samplingIndex:" + g),
                                    /firefox/i.test(h)
                                        ? g >= 6
                                            ? ((s = 5), (u = new Array(4)), (o = g - 3))
                                            : ((s = 2), (u = new Array(2)), (o = g))
                                        : -1 !== h.indexOf("android")
                                        ? ((s = 2), (u = new Array(2)), (o = g))
                                        : ((s = 5),
                                          (u = new Array(4)),
                                          (a && (-1 !== a.indexOf("mp4a.40.29") || -1 !== a.indexOf("mp4a.40.5"))) || (!a && g >= 6)
                                              ? (o = g - 3)
                                              : (((a && -1 !== a.indexOf("mp4a.40.2") && ((g >= 6 && 1 === l) || /vivaldi/i.test(h))) || (!a && 1 === l)) && ((s = 2), (u = new Array(2))), (o = g))),
                                    (u[0] = s << 3),
                                    (u[0] |= (14 & g) >> 1),
                                    (u[1] |= (1 & g) << 7),
                                    (u[1] |= l << 3),
                                    5 === s && ((u[1] |= (14 & o) >> 1), (u[2] = (1 & o) << 7), (u[2] |= 8), (u[3] = 0)),
                                    { config: u, samplerate: c[g], channelCount: l, codec: "mp4a.40." + s, manifestCodec: d }
                                );
                            t.trigger(i.a.ERROR, { type: n.b.MEDIA_ERROR, details: n.a.FRAG_PARSING_ERROR, fatal: !0, reason: "invalid ADTS sampling index:" + g });
                        })(e, r, a, s);
                        if (!o) return;
                        (t.config = o.config),
                            (t.samplerate = o.samplerate),
                            (t.channelCount = o.channelCount),
                            (t.codec = o.codec),
                            (t.manifestCodec = o.manifestCodec),
                            f.b.log("parsed codec:" + t.codec + ", rate:" + o.samplerate + ", channels:" + o.channelCount);
                    }
                }
                function b(t) {
                    return 9216e4 / t;
                }
                function E(t, e, r, i, n) {
                    var a = (function (t, e, r, i, n) {
                        var a = v(t, e),
                            s = p(t, e);
                        if ((s -= a) > 0) return { headerLength: a, frameLength: s, stamp: r + i * n };
                    })(e, r, i, n, b(t.samplerate));
                    if (a) {
                        var s,
                            o = a.frameLength,
                            l = a.headerLength,
                            u = a.stamp,
                            h = l + o,
                            d = Math.max(0, r + h - e.length);
                        d ? (s = new Uint8Array(h - l)).set(e.subarray(r + l, e.length), 0) : (s = e.subarray(r + l, r + h));
                        var c = { unit: s, pts: u };
                        return d || t.samples.push(c), { sample: c, length: h, missing: d };
                    }
                }
                function S(t, e) {
                    return (S =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var L = (function (t) {
                    var e, r;
                    function i(e, r) {
                        var i;
                        return ((i = t.call(this) || this).observer = void 0), (i.config = void 0), (i.observer = e), (i.config = r), i;
                    }
                    (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), S(e, r);
                    var n = i.prototype;
                    return (
                        (n.resetInitSegment = function (e, r, i) {
                            t.prototype.resetInitSegment.call(this, e, r, i),
                                (this._audioTrack = { container: "audio/adts", type: "audio", id: 0, pid: -1, sequenceNumber: 0, isAAC: !0, samples: [], manifestCodec: e, duration: i, inputTimeScale: 9e4, dropped: 0 });
                        }),
                        (i.probe = function (t) {
                            if (!t) return !1;
                            for (var e = (o.b(t, 0) || []).length, r = t.length; e < r; e++) if (y(t, e)) return f.b.log("ADTS sync word found !"), !0;
                            return !1;
                        }),
                        (n.canParse = function (t, e) {
                            return (function (t, e) {
                                return (
                                    (function (t, e) {
                                        return e + 5 < t.length;
                                    })(t, e) &&
                                    g(t, e) &&
                                    p(t, e) <= t.length - e
                                );
                            })(t, e);
                        }),
                        (n.appendFrame = function (t, e, r) {
                            T(t, this.observer, e, r, t.manifestCodec);
                            var i = E(t, e, r, this.initPTS, this.frameIndex);
                            if (i && 0 === i.missing) return i;
                        }),
                        i
                    );
                })(c);
                L.minProbeByteLength = 9;
                var A = L,
                    R = (function () {
                        function t(t, e) {
                            (this.remainderData = null), (this.config = void 0), (this.config = e);
                        }
                        var e = t.prototype;
                        return (
                            (e.resetTimeStamp = function () {}),
                            (e.resetInitSegment = function () {}),
                            (e.resetContiguity = function () {}),
                            (t.probe = function (t) {
                                return Object(l.b)({ data: t, start: 0, end: Math.min(t.length, 16384) }, ["moof"]).length > 0;
                            }),
                            (e.demux = function (t) {
                                var e = t,
                                    r = { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 };
                                if (this.config.progressive) {
                                    this.remainderData && (e = Object(l.a)(this.remainderData, t));
                                    var i = Object(l.h)(e);
                                    (this.remainderData = i.remainder), (r.samples = i.valid || new Uint8Array());
                                } else r.samples = e;
                                return {
                                    audioTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                    avcTrack: r,
                                    id3Track: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                    textTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                };
                            }),
                            (e.flush = function () {
                                var t = { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 };
                                return (
                                    (t.samples = this.remainderData || new Uint8Array()),
                                    (this.remainderData = null),
                                    {
                                        audioTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                        avcTrack: t,
                                        id3Track: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                        textTrack: { type: "", id: -1, pid: -1, inputTimeScale: 9e4, sequenceNumber: -1, samples: [], dropped: 0 },
                                    }
                                );
                            }),
                            (e.demuxSampleAes = function (t, e, r) {
                                return Promise.reject(new Error("The MP4 demuxer does not support SAMPLE-AES decryption"));
                            }),
                            (e.destroy = function () {}),
                            t
                        );
                    })();
                R.minProbeByteLength = 1024;
                var D = R,
                    k = null,
                    _ = [
                        32,
                        64,
                        96,
                        128,
                        160,
                        192,
                        224,
                        256,
                        288,
                        320,
                        352,
                        384,
                        416,
                        448,
                        32,
                        48,
                        56,
                        64,
                        80,
                        96,
                        112,
                        128,
                        160,
                        192,
                        224,
                        256,
                        320,
                        384,
                        32,
                        40,
                        48,
                        56,
                        64,
                        80,
                        96,
                        112,
                        128,
                        160,
                        192,
                        224,
                        256,
                        320,
                        32,
                        48,
                        56,
                        64,
                        80,
                        96,
                        112,
                        128,
                        144,
                        160,
                        176,
                        192,
                        224,
                        256,
                        8,
                        16,
                        24,
                        32,
                        40,
                        48,
                        56,
                        64,
                        80,
                        96,
                        112,
                        128,
                        144,
                        160,
                    ],
                    I = [44100, 48e3, 32e3, 22050, 24e3, 16e3, 11025, 12e3, 8e3],
                    C = [
                        [0, 72, 144, 12],
                        [0, 0, 0, 0],
                        [0, 72, 144, 12],
                        [0, 144, 144, 12],
                    ],
                    w = [0, 1, 1, 4];
                function O(t, e, r, i, n) {
                    if (!(r + 24 > e.length)) {
                        var a = x(e, r);
                        if (a && r + a.frameLength <= e.length) {
                            var s = i + n * ((9e4 * a.samplesPerFrame) / a.sampleRate),
                                o = { unit: e.subarray(r, r + a.frameLength), pts: s, dts: s };
                            return (t.config = []), (t.channelCount = a.channelCount), (t.samplerate = a.sampleRate), t.samples.push(o), { sample: o, length: a.frameLength, missing: 0 };
                        }
                    }
                }
                function x(t, e) {
                    var r = (t[e + 1] >> 3) & 3,
                        i = (t[e + 1] >> 1) & 3,
                        n = (t[e + 2] >> 4) & 15,
                        a = (t[e + 2] >> 2) & 3;
                    if (1 !== r && 0 !== n && 15 !== n && 3 !== a) {
                        var s = (t[e + 2] >> 1) & 1,
                            o = t[e + 3] >> 6,
                            l = 1e3 * _[14 * (3 === r ? 3 - i : 3 === i ? 3 : 4) + n - 1],
                            u = I[3 * (3 === r ? 0 : 2 === r ? 1 : 2) + a],
                            h = 3 === o ? 1 : 2,
                            d = C[r][i],
                            c = w[i],
                            f = 8 * d * c,
                            g = Math.floor((d * l) / u + s) * c;
                        if (null === k) {
                            var v = (navigator.userAgent || "").match(/Chrome\/(\d+)/i);
                            k = v ? parseInt(v[1]) : 0;
                        }
                        return !!k && k <= 87 && 2 === i && l >= 224e3 && 0 === o && (t[e + 3] = 128 | t[e + 3]), { sampleRate: u, channelCount: h, frameLength: g, samplesPerFrame: f };
                    }
                }
                function P(t, e) {
                    return 255 === t[e] && 224 == (224 & t[e + 1]) && 0 != (6 & t[e + 1]);
                }
                function M(t, e) {
                    return e + 1 < t.length && P(t, e);
                }
                function F(t, e) {
                    if (e + 1 < t.length && P(t, e)) {
                        var r = x(t, e),
                            i = 4;
                        null != r && r.frameLength && (i = r.frameLength);
                        var n = e + i;
                        return n === t.length || M(t, n);
                    }
                    return !1;
                }
                var N = (function () {
                        function t(t) {
                            (this.data = void 0), (this.bytesAvailable = void 0), (this.word = void 0), (this.bitsAvailable = void 0), (this.data = t), (this.bytesAvailable = t.byteLength), (this.word = 0), (this.bitsAvailable = 0);
                        }
                        var e = t.prototype;
                        return (
                            (e.loadWord = function () {
                                var t = this.data,
                                    e = this.bytesAvailable,
                                    r = t.byteLength - e,
                                    i = new Uint8Array(4),
                                    n = Math.min(4, e);
                                if (0 === n) throw new Error("no bytes available");
                                i.set(t.subarray(r, r + n)), (this.word = new DataView(i.buffer).getUint32(0)), (this.bitsAvailable = 8 * n), (this.bytesAvailable -= n);
                            }),
                            (e.skipBits = function (t) {
                                var e;
                                this.bitsAvailable > t
                                    ? ((this.word <<= t), (this.bitsAvailable -= t))
                                    : ((t -= this.bitsAvailable), (t -= (e = t >> 3) >> 3), (this.bytesAvailable -= e), this.loadWord(), (this.word <<= t), (this.bitsAvailable -= t));
                            }),
                            (e.readBits = function (t) {
                                var e = Math.min(this.bitsAvailable, t),
                                    r = this.word >>> (32 - e);
                                return (
                                    t > 32 && f.b.error("Cannot read more than 32 bits at a time"),
                                    (this.bitsAvailable -= e),
                                    this.bitsAvailable > 0 ? (this.word <<= e) : this.bytesAvailable > 0 && this.loadWord(),
                                    (e = t - e) > 0 && this.bitsAvailable ? (r << e) | this.readBits(e) : r
                                );
                            }),
                            (e.skipLZ = function () {
                                var t;
                                for (t = 0; t < this.bitsAvailable; ++t) if (0 != (this.word & (2147483648 >>> t))) return (this.word <<= t), (this.bitsAvailable -= t), t;
                                return this.loadWord(), t + this.skipLZ();
                            }),
                            (e.skipUEG = function () {
                                this.skipBits(1 + this.skipLZ());
                            }),
                            (e.skipEG = function () {
                                this.skipBits(1 + this.skipLZ());
                            }),
                            (e.readUEG = function () {
                                var t = this.skipLZ();
                                return this.readBits(t + 1) - 1;
                            }),
                            (e.readEG = function () {
                                var t = this.readUEG();
                                return 1 & t ? (1 + t) >>> 1 : -1 * (t >>> 1);
                            }),
                            (e.readBoolean = function () {
                                return 1 === this.readBits(1);
                            }),
                            (e.readUByte = function () {
                                return this.readBits(8);
                            }),
                            (e.readUShort = function () {
                                return this.readBits(16);
                            }),
                            (e.readUInt = function () {
                                return this.readBits(32);
                            }),
                            (e.skipScalingList = function (t) {
                                for (var e = 8, r = 8, i = 0; i < t; i++) 0 !== r && (r = (e + this.readEG() + 256) % 256), (e = 0 === r ? e : r);
                            }),
                            (e.readSPS = function () {
                                var t,
                                    e,
                                    r,
                                    i = 0,
                                    n = 0,
                                    a = 0,
                                    s = 0,
                                    o = this.readUByte.bind(this),
                                    l = this.readBits.bind(this),
                                    u = this.readUEG.bind(this),
                                    h = this.readBoolean.bind(this),
                                    d = this.skipBits.bind(this),
                                    c = this.skipEG.bind(this),
                                    f = this.skipUEG.bind(this),
                                    g = this.skipScalingList.bind(this);
                                o();
                                var v = o();
                                if ((l(5), d(3), o(), f(), 100 === v || 110 === v || 122 === v || 244 === v || 44 === v || 83 === v || 86 === v || 118 === v || 128 === v)) {
                                    var p = u();
                                    if ((3 === p && d(1), f(), f(), d(1), h())) for (e = 3 !== p ? 8 : 12, r = 0; r < e; r++) h() && g(r < 6 ? 16 : 64);
                                }
                                f();
                                var m = u();
                                if (0 === m) u();
                                else if (1 === m) for (d(1), c(), c(), t = u(), r = 0; r < t; r++) c();
                                f(), d(1);
                                var y = u(),
                                    T = u(),
                                    b = l(1);
                                0 === b && d(1), d(1), h() && ((i = u()), (n = u()), (a = u()), (s = u()));
                                var E = [1, 1];
                                if (h() && h())
                                    switch (o()) {
                                        case 1:
                                            E = [1, 1];
                                            break;
                                        case 2:
                                            E = [12, 11];
                                            break;
                                        case 3:
                                            E = [10, 11];
                                            break;
                                        case 4:
                                            E = [16, 11];
                                            break;
                                        case 5:
                                            E = [40, 33];
                                            break;
                                        case 6:
                                            E = [24, 11];
                                            break;
                                        case 7:
                                            E = [20, 11];
                                            break;
                                        case 8:
                                            E = [32, 11];
                                            break;
                                        case 9:
                                            E = [80, 33];
                                            break;
                                        case 10:
                                            E = [18, 11];
                                            break;
                                        case 11:
                                            E = [15, 11];
                                            break;
                                        case 12:
                                            E = [64, 33];
                                            break;
                                        case 13:
                                            E = [160, 99];
                                            break;
                                        case 14:
                                            E = [4, 3];
                                            break;
                                        case 15:
                                            E = [3, 2];
                                            break;
                                        case 16:
                                            E = [2, 1];
                                            break;
                                        case 255:
                                            E = [(o() << 8) | o(), (o() << 8) | o()];
                                    }
                                return { width: Math.ceil(16 * (y + 1) - 2 * i - 2 * n), height: (2 - b) * (T + 1) * 16 - (b ? 2 : 4) * (a + s), pixelRatio: E };
                            }),
                            (e.readSliceType = function () {
                                return this.readUByte(), this.readUEG(), this.readUEG();
                            }),
                            t
                        );
                    })(),
                    U = (function () {
                        function t(t, e, r) {
                            (this.keyData = void 0), (this.decrypter = void 0), (this.keyData = r), (this.decrypter = new a.a(t, e, { removePKCS7Padding: !1 }));
                        }
                        var e = t.prototype;
                        return (
                            (e.decryptBuffer = function (t, e) {
                                this.decrypter.decrypt(t, this.keyData.key.buffer, this.keyData.iv.buffer, e);
                            }),
                            (e.decryptAacSample = function (t, e, r, i) {
                                var n = t[e].unit,
                                    a = n.subarray(16, n.length - (n.length % 16)),
                                    s = a.buffer.slice(a.byteOffset, a.byteOffset + a.length),
                                    o = this;
                                this.decryptBuffer(s, function (a) {
                                    var s = new Uint8Array(a);
                                    n.set(s, 16), i || o.decryptAacSamples(t, e + 1, r);
                                });
                            }),
                            (e.decryptAacSamples = function (t, e, r) {
                                for (; ; e++) {
                                    if (e >= t.length) return void r();
                                    if (!(t[e].unit.length < 32)) {
                                        var i = this.decrypter.isSync();
                                        if ((this.decryptAacSample(t, e, r, i), !i)) return;
                                    }
                                }
                            }),
                            (e.getAvcEncryptedData = function (t) {
                                for (var e = 16 * Math.floor((t.length - 48) / 160) + 16, r = new Int8Array(e), i = 0, n = 32; n <= t.length - 16; n += 160, i += 16) r.set(t.subarray(n, n + 16), i);
                                return r;
                            }),
                            (e.getAvcDecryptedUnit = function (t, e) {
                                for (var r = new Uint8Array(e), i = 0, n = 32; n <= t.length - 16; n += 160, i += 16) t.set(r.subarray(i, i + 16), n);
                                return t;
                            }),
                            (e.decryptAvcSample = function (t, e, r, i, n, a) {
                                var s = q(n.data),
                                    o = this.getAvcEncryptedData(s),
                                    l = this;
                                this.decryptBuffer(o.buffer, function (o) {
                                    (n.data = l.getAvcDecryptedUnit(s, o)), a || l.decryptAvcSamples(t, e, r + 1, i);
                                });
                            }),
                            (e.decryptAvcSamples = function (t, e, r, i) {
                                if (t instanceof Uint8Array) throw new Error("Cannot decrypt samples of type Uint8Array");
                                for (; ; e++, r = 0) {
                                    if (e >= t.length) return void i();
                                    for (var n = t[e].units; !(r >= n.length); r++) {
                                        var a = n[r];
                                        if (!(a.data.length <= 48 || (1 !== a.type && 5 !== a.type))) {
                                            var s = this.decrypter.isSync();
                                            if ((this.decryptAvcSample(t, e, r, i, a, s), !s)) return;
                                        }
                                    }
                                }
                            }),
                            t
                        );
                    })(),
                    B = { video: 1, audio: 2, id3: 3, text: 4 },
                    G = (function () {
                        function t(t, e, r) {
                            (this.observer = void 0),
                                (this.config = void 0),
                                (this.typeSupported = void 0),
                                (this.sampleAes = null),
                                (this.pmtParsed = !1),
                                (this.audioCodec = void 0),
                                (this.videoCodec = void 0),
                                (this._duration = 0),
                                (this.aacLastPTS = null),
                                (this._initPTS = null),
                                (this._initDTS = null),
                                (this._pmtId = -1),
                                (this._avcTrack = void 0),
                                (this._audioTrack = void 0),
                                (this._id3Track = void 0),
                                (this._txtTrack = void 0),
                                (this.aacOverFlow = null),
                                (this.avcSample = null),
                                (this.remainderData = null),
                                (this.observer = t),
                                (this.config = e),
                                (this.typeSupported = r);
                        }
                        (t.probe = function (e) {
                            var r = t.syncOffset(e);
                            return !(r < 0) && (r && f.b.warn("MPEG2-TS detected but first sync word found @ offset " + r + ", junk ahead ?"), !0);
                        }),
                            (t.syncOffset = function (t) {
                                for (var e = Math.min(1e3, t.length - 564), r = 0; r < e; ) {
                                    if (71 === t[r] && 71 === t[r + 188] && 71 === t[r + 376]) return r;
                                    r++;
                                }
                                return -1;
                            }),
                            (t.createTrack = function (t, e) {
                                return { container: "video" === t || "audio" === t ? "video/mp2t" : void 0, type: t, id: B[t], pid: -1, inputTimeScale: 9e4, sequenceNumber: 0, samples: [], dropped: 0, duration: "audio" === t ? e : void 0 };
                            });
                        var e = t.prototype;
                        return (
                            (e.resetInitSegment = function (e, r, i) {
                                (this.pmtParsed = !1),
                                    (this._pmtId = -1),
                                    (this._avcTrack = t.createTrack("video", i)),
                                    (this._audioTrack = t.createTrack("audio", i)),
                                    (this._id3Track = t.createTrack("id3", i)),
                                    (this._txtTrack = t.createTrack("text", i)),
                                    (this._audioTrack.isAAC = !0),
                                    (this.aacOverFlow = null),
                                    (this.aacLastPTS = null),
                                    (this.avcSample = null),
                                    (this.audioCodec = e),
                                    (this.videoCodec = r),
                                    (this._duration = i);
                            }),
                            (e.resetTimeStamp = function () {}),
                            (e.resetContiguity = function () {
                                var t = this._audioTrack,
                                    e = this._avcTrack,
                                    r = this._id3Track;
                                t && (t.pesData = null), e && (e.pesData = null), r && (r.pesData = null), (this.aacOverFlow = null), (this.aacLastPTS = null);
                            }),
                            (e.demux = function (e, r, a, s) {
                                var o;
                                void 0 === a && (a = !1), void 0 === s && (s = !1), a || (this.sampleAes = null);
                                var u = this._avcTrack,
                                    h = this._audioTrack,
                                    d = this._id3Track,
                                    c = u.pid,
                                    g = u.pesData,
                                    v = h.pid,
                                    p = d.pid,
                                    m = h.pesData,
                                    y = d.pesData,
                                    T = !1,
                                    b = this.pmtParsed,
                                    E = this._pmtId,
                                    S = e.length;
                                if ((this.remainderData && ((S = (e = Object(l.a)(this.remainderData, e)).length), (this.remainderData = null)), S < 188 && !s))
                                    return (this.remainderData = e), { audioTrack: h, avcTrack: u, id3Track: d, textTrack: this._txtTrack };
                                var L = Math.max(0, t.syncOffset(e));
                                (S -= (S + L) % 188) < e.byteLength && !s && (this.remainderData = new Uint8Array(e.buffer, S, e.buffer.byteLength - S));
                                for (var A = L; A < S; A += 188)
                                    if (71 === e[A]) {
                                        var R = !!(64 & e[A + 1]),
                                            D = ((31 & e[A + 1]) << 8) + e[A + 2],
                                            k = void 0;
                                        if ((48 & e[A + 3]) >> 4 > 1) {
                                            if ((k = A + 5 + e[A + 4]) === A + 188) continue;
                                        } else k = A + 4;
                                        switch (D) {
                                            case c:
                                                R && (g && (o = V(g)) && this.parseAVCPES(o, !1), (g = { data: [], size: 0 })), g && (g.data.push(e.subarray(k, A + 188)), (g.size += A + 188 - k));
                                                break;
                                            case v:
                                                R && (m && (o = V(m)) && (h.isAAC ? this.parseAACPES(o) : this.parseMPEGPES(o)), (m = { data: [], size: 0 })), m && (m.data.push(e.subarray(k, A + 188)), (m.size += A + 188 - k));
                                                break;
                                            case p:
                                                R && (y && (o = V(y)) && this.parseID3PES(o), (y = { data: [], size: 0 })), y && (y.data.push(e.subarray(k, A + 188)), (y.size += A + 188 - k));
                                                break;
                                            case 0:
                                                R && (k += e[k] + 1), (E = this._pmtId = j(e, k));
                                                break;
                                            case E:
                                                R && (k += e[k] + 1);
                                                var _ = H(e, k, !0 === this.typeSupported.mpeg || !0 === this.typeSupported.mp3, a);
                                                (c = _.avc) > 0 && (u.pid = c),
                                                    (v = _.audio) > 0 && ((h.pid = v), (h.isAAC = _.isAAC)),
                                                    (p = _.id3) > 0 && (d.pid = p),
                                                    T && !b && (f.b.log("reparse from beginning"), (T = !1), (A = L - 188)),
                                                    (b = this.pmtParsed = !0);
                                                break;
                                            case 17:
                                            case 8191:
                                                break;
                                            default:
                                                T = !0;
                                        }
                                    } else this.observer.emit(i.a.ERROR, i.a.ERROR, { type: n.b.MEDIA_ERROR, details: n.a.FRAG_PARSING_ERROR, fatal: !1, reason: "TS packet did not start with 0x47" });
                                (u.pesData = g), (h.pesData = m), (d.pesData = y);
                                var I = { audioTrack: h, avcTrack: u, id3Track: d, textTrack: this._txtTrack };
                                return s && this.extractRemainingSamples(I), I;
                            }),
                            (e.flush = function () {
                                var t,
                                    e = this.remainderData;
                                return (
                                    (this.remainderData = null),
                                    (t = e ? this.demux(e, -1, !1, !0) : { audioTrack: this._audioTrack, avcTrack: this._avcTrack, textTrack: this._txtTrack, id3Track: this._id3Track }),
                                    this.extractRemainingSamples(t),
                                    this.sampleAes ? this.decrypt(t, this.sampleAes) : t
                                );
                            }),
                            (e.extractRemainingSamples = function (t) {
                                var e,
                                    r = t.audioTrack,
                                    i = t.avcTrack,
                                    n = t.id3Track,
                                    a = i.pesData,
                                    s = r.pesData,
                                    o = n.pesData;
                                a && (e = V(a)) ? (this.parseAVCPES(e, !0), (i.pesData = null)) : (i.pesData = a),
                                    s && (e = V(s))
                                        ? (r.isAAC ? this.parseAACPES(e) : this.parseMPEGPES(e), (r.pesData = null))
                                        : (null != s && s.size && f.b.log("last AAC PES packet truncated,might overlap between fragments"), (r.pesData = s)),
                                    o && (e = V(o)) ? (this.parseID3PES(e), (n.pesData = null)) : (n.pesData = o);
                            }),
                            (e.demuxSampleAes = function (t, e, r) {
                                var i = this.demux(t, r, !0, !this.config.progressive),
                                    n = (this.sampleAes = new U(this.observer, this.config, e));
                                return this.decrypt(i, n);
                            }),
                            (e.decrypt = function (t, e) {
                                return new Promise(function (r) {
                                    var i = t.audioTrack,
                                        n = t.avcTrack;
                                    i.samples && i.isAAC
                                        ? e.decryptAacSamples(i.samples, 0, function () {
                                              n.samples
                                                  ? e.decryptAvcSamples(n.samples, 0, 0, function () {
                                                        r(t);
                                                    })
                                                  : r(t);
                                          })
                                        : n.samples &&
                                          e.decryptAvcSamples(n.samples, 0, 0, function () {
                                              r(t);
                                          });
                                });
                            }),
                            (e.destroy = function () {
                                (this._initPTS = this._initDTS = null), (this._duration = 0);
                            }),
                            (e.parseAVCPES = function (t, e) {
                                var r,
                                    i = this,
                                    n = this._avcTrack,
                                    a = this.parseAVCNALu(t.data),
                                    s = this.avcSample,
                                    l = !1;
                                (t.data = null),
                                    s && a.length && !n.audFound && (W(s, n), (s = this.avcSample = K(!1, t.pts, t.dts, ""))),
                                    a.forEach(function (e) {
                                        switch (e.type) {
                                            case 1:
                                                (r = !0), s || (s = i.avcSample = K(!0, t.pts, t.dts, "")), (s.frame = !0);
                                                var a = e.data;
                                                if (l && a.length > 4) {
                                                    var u = new N(a).readSliceType();
                                                    (2 !== u && 4 !== u && 7 !== u && 9 !== u) || (s.key = !0);
                                                }
                                                break;
                                            case 5:
                                                (r = !0), s || (s = i.avcSample = K(!0, t.pts, t.dts, "")), (s.key = !0), (s.frame = !0);
                                                break;
                                            case 6:
                                                r = !0;
                                                var h = new N(q(e.data));
                                                h.readUByte();
                                                for (var d = 0, c = 0, f = !1, g = 0; !f && h.bytesAvailable > 1; ) {
                                                    d = 0;
                                                    do {
                                                        d += g = h.readUByte();
                                                    } while (255 === g);
                                                    c = 0;
                                                    do {
                                                        c += g = h.readUByte();
                                                    } while (255 === g);
                                                    if (4 === d && 0 !== h.bytesAvailable) {
                                                        if (((f = !0), 181 === h.readUByte()))
                                                            if (49 === h.readUShort())
                                                                if (1195456820 === h.readUInt())
                                                                    if (3 === h.readUByte()) {
                                                                        for (var v = h.readUByte(), p = 31 & v, m = [v, h.readUByte()], y = 0; y < p; y++) m.push(h.readUByte()), m.push(h.readUByte()), m.push(h.readUByte());
                                                                        Y(i._txtTrack.samples, { type: 3, pts: t.pts, bytes: m });
                                                                    }
                                                    } else if (5 === d && 0 !== h.bytesAvailable) {
                                                        if (((f = !0), c > 16)) {
                                                            for (var T = [], b = 0; b < 16; b++) T.push(h.readUByte().toString(16)), (3 !== b && 5 !== b && 7 !== b && 9 !== b) || T.push("-");
                                                            for (var E = c - 16, S = new Uint8Array(E), L = 0; L < E; L++) S[L] = h.readUByte();
                                                            Y(i._txtTrack.samples, { pts: t.pts, payloadType: d, uuid: T.join(""), userData: Object(o.f)(S), userDataBytes: S });
                                                        }
                                                    } else if (c < h.bytesAvailable) for (var A = 0; A < c; A++) h.readUByte();
                                                }
                                                break;
                                            case 7:
                                                if (((r = !0), (l = !0), !n.sps)) {
                                                    var R = new N(e.data).readSPS();
                                                    (n.width = R.width), (n.height = R.height), (n.pixelRatio = R.pixelRatio), (n.sps = [e.data]), (n.duration = i._duration);
                                                    for (var D = e.data.subarray(1, 4), k = "avc1.", _ = 0; _ < 3; _++) {
                                                        var I = D[_].toString(16);
                                                        I.length < 2 && (I = "0" + I), (k += I);
                                                    }
                                                    n.codec = k;
                                                }
                                                break;
                                            case 8:
                                                (r = !0), n.pps || (n.pps = [e.data]);
                                                break;
                                            case 9:
                                                (r = !1), (n.audFound = !0), s && W(s, n), (s = i.avcSample = K(!1, t.pts, t.dts, ""));
                                                break;
                                            case 12:
                                                r = !1;
                                                break;
                                            default:
                                                (r = !1), s && (s.debug += "unknown NAL " + e.type + " ");
                                        }
                                        s && r && s.units.push(e);
                                    }),
                                    e && s && (W(s, n), (this.avcSample = null));
                            }),
                            (e.getLastNalUnit = function () {
                                var t,
                                    e,
                                    r = this.avcSample;
                                if (!r || 0 === r.units.length) {
                                    var i = this._avcTrack.samples;
                                    r = i[i.length - 1];
                                }
                                if (null !== (t = r) && void 0 !== t && t.units) {
                                    var n = r.units;
                                    e = n[n.length - 1];
                                }
                                return e;
                            }),
                            (e.parseAVCNALu = function (t) {
                                var e,
                                    r,
                                    i = t.byteLength,
                                    n = this._avcTrack,
                                    a = n.naluState || 0,
                                    s = a,
                                    o = [],
                                    l = 0,
                                    u = -1,
                                    h = 0;
                                for (-1 === a && ((u = 0), (h = 31 & t[0]), (a = 0), (l = 1)); l < i; )
                                    if (((e = t[l++]), a))
                                        if (1 !== a)
                                            if (e)
                                                if (1 === e) {
                                                    if (u >= 0) {
                                                        var d = { data: t.subarray(u, l - a - 1), type: h };
                                                        o.push(d);
                                                    } else {
                                                        var c = this.getLastNalUnit();
                                                        if (c && (s && l <= 4 - s && c.state && (c.data = c.data.subarray(0, c.data.byteLength - s)), (r = l - a - 1) > 0)) {
                                                            var f = new Uint8Array(c.data.byteLength + r);
                                                            f.set(c.data, 0), f.set(t.subarray(0, r), c.data.byteLength), (c.data = f);
                                                        }
                                                    }
                                                    l < i ? ((u = l), (h = 31 & t[l]), (a = 0)) : (a = -1);
                                                } else a = 0;
                                            else a = 3;
                                        else a = e ? 0 : 2;
                                    else a = e ? 0 : 1;
                                if (u >= 0 && a >= 0) {
                                    var g = { data: t.subarray(u, i), type: h, state: a };
                                    o.push(g);
                                }
                                if (0 === o.length) {
                                    var v = this.getLastNalUnit();
                                    if (v) {
                                        var p = new Uint8Array(v.data.byteLength + t.byteLength);
                                        p.set(v.data, 0), p.set(t, v.data.byteLength), (v.data = p);
                                    }
                                }
                                return (n.naluState = a), o;
                            }),
                            (e.parseAACPES = function (t) {
                                var e,
                                    r,
                                    a,
                                    s,
                                    o,
                                    l = 0,
                                    u = this._audioTrack,
                                    h = this.aacOverFlow,
                                    d = t.data;
                                if (h) {
                                    this.aacOverFlow = null;
                                    var c = h.sample.unit.byteLength,
                                        g = Math.min(h.missing, c),
                                        v = c - g;
                                    h.sample.unit.set(d.subarray(0, g), v), u.samples.push(h.sample), (l = h.missing);
                                }
                                for (e = l, r = d.length; e < r - 1 && !m(d, e); e++);
                                if (
                                    e !== l &&
                                    (e < r - 1 ? ((a = "AAC PES did not start with ADTS header,offset:" + e), (s = !1)) : ((a = "no ADTS header found in AAC PES"), (s = !0)),
                                    f.b.warn("parsing error:" + a),
                                    this.observer.emit(i.a.ERROR, i.a.ERROR, { type: n.b.MEDIA_ERROR, details: n.a.FRAG_PARSING_ERROR, fatal: s, reason: a }),
                                    s)
                                )
                                    return;
                                if ((T(u, this.observer, d, e, this.audioCodec), void 0 !== t.pts)) o = t.pts;
                                else {
                                    if (!h) return void f.b.warn("[tsdemuxer]: AAC PES unknown PTS");
                                    var p = b(u.samplerate);
                                    o = h.sample.pts + p;
                                }
                                for (var y = 0; e < r; ) {
                                    if (m(d, e)) {
                                        if (e + 5 < r) {
                                            var S = E(u, d, e, o, y);
                                            if (S) {
                                                if (!S.missing) {
                                                    (e += S.length), y++;
                                                    continue;
                                                }
                                                this.aacOverFlow = S;
                                            }
                                        }
                                        break;
                                    }
                                    e++;
                                }
                            }),
                            (e.parseMPEGPES = function (t) {
                                var e = t.data,
                                    r = e.length,
                                    i = 0,
                                    n = 0,
                                    a = t.pts;
                                if (void 0 !== a)
                                    for (; n < r; )
                                        if (M(e, n)) {
                                            var s = O(this._audioTrack, e, n, a, i);
                                            if (!s) break;
                                            (n += s.length), i++;
                                        } else n++;
                                else f.b.warn("[tsdemuxer]: MPEG PES unknown PTS");
                            }),
                            (e.parseID3PES = function (t) {
                                void 0 !== t.pts ? this._id3Track.samples.push(t) : f.b.warn("[tsdemuxer]: ID3 PES unknown PTS");
                            }),
                            t
                        );
                    })();
                function K(t, e, r, i) {
                    return { key: t, frame: !1, pts: e, dts: r, units: [], debug: i, length: 0 };
                }
                function j(t, e) {
                    return ((31 & t[e + 10]) << 8) | t[e + 11];
                }
                function H(t, e, r, i) {
                    var n = { audio: -1, avc: -1, id3: -1, isAAC: !0 },
                        a = e + 3 + (((15 & t[e + 1]) << 8) | t[e + 2]) - 4;
                    for (e += 12 + (((15 & t[e + 10]) << 8) | t[e + 11]); e < a; ) {
                        var s = ((31 & t[e + 1]) << 8) | t[e + 2];
                        switch (t[e]) {
                            case 207:
                                if (!i) {
                                    f.b.log("ADTS AAC with AES-128-CBC frame encryption found in unencrypted stream");
                                    break;
                                }
                            case 15:
                                -1 === n.audio && (n.audio = s);
                                break;
                            case 21:
                                -1 === n.id3 && (n.id3 = s);
                                break;
                            case 219:
                                if (!i) {
                                    f.b.log("H.264 with AES-128-CBC slice encryption found in unencrypted stream");
                                    break;
                                }
                            case 27:
                                -1 === n.avc && (n.avc = s);
                                break;
                            case 3:
                            case 4:
                                r ? -1 === n.audio && ((n.audio = s), (n.isAAC = !1)) : f.b.log("MPEG audio found, not supported in this browser");
                                break;
                            case 36:
                                f.b.warn("Unsupported HEVC stream type found");
                        }
                        e += 5 + (((15 & t[e + 3]) << 8) | t[e + 4]);
                    }
                    return n;
                }
                function V(t) {
                    var e,
                        r,
                        i,
                        n,
                        a,
                        s = 0,
                        o = t.data;
                    if (!t || 0 === t.size) return null;
                    for (; o[0].length < 19 && o.length > 1; ) {
                        var l = new Uint8Array(o[0].length + o[1].length);
                        l.set(o[0]), l.set(o[1], o[0].length), (o[0] = l), o.splice(1, 1);
                    }
                    if (1 === ((e = o[0])[0] << 16) + (e[1] << 8) + e[2]) {
                        if ((r = (e[4] << 8) + e[5]) && r > t.size - 6) return null;
                        var u = e[7];
                        192 & u &&
                            ((n = 536870912 * (14 & e[9]) + 4194304 * (255 & e[10]) + 16384 * (254 & e[11]) + 128 * (255 & e[12]) + (254 & e[13]) / 2),
                            64 & u
                                ? n - (a = 536870912 * (14 & e[14]) + 4194304 * (255 & e[15]) + 16384 * (254 & e[16]) + 128 * (255 & e[17]) + (254 & e[18]) / 2) > 54e5 &&
                                  (f.b.warn(Math.round((n - a) / 9e4) + "s delta between PTS and DTS, align them"), (n = a))
                                : (a = n));
                        var h = (i = e[8]) + 9;
                        if (t.size <= h) return null;
                        t.size -= h;
                        for (var d = new Uint8Array(t.size), c = 0, g = o.length; c < g; c++) {
                            var v = (e = o[c]).byteLength;
                            if (h) {
                                if (h > v) {
                                    h -= v;
                                    continue;
                                }
                                (e = e.subarray(h)), (v -= h), (h = 0);
                            }
                            d.set(e, s), (s += v);
                        }
                        return r && (r -= i + 3), { data: d, pts: n, dts: a, len: r };
                    }
                    return null;
                }
                function W(t, e) {
                    if (t.units.length && t.frame) {
                        if (void 0 === t.pts) {
                            var r = e.samples,
                                i = r.length;
                            if (!i) return void e.dropped++;
                            var n = r[i - 1];
                            (t.pts = n.pts), (t.dts = n.dts);
                        }
                        e.samples.push(t);
                    }
                    t.debug.length && f.b.log(t.pts + "/" + t.dts + ":" + t.debug);
                }
                function Y(t, e) {
                    var r = t.length;
                    if (r > 0) {
                        if (e.pts >= t[r - 1].pts) t.push(e);
                        else
                            for (var i = r - 1; i >= 0; i--)
                                if (e.pts < t[i].pts) {
                                    t.splice(i, 0, e);
                                    break;
                                }
                    } else t.push(e);
                }
                function q(t) {
                    for (var e = t.byteLength, r = [], i = 1; i < e - 2; ) 0 === t[i] && 0 === t[i + 1] && 3 === t[i + 2] ? (r.push(i + 2), (i += 2)) : i++;
                    if (0 === r.length) return t;
                    var n = e - r.length,
                        a = new Uint8Array(n),
                        s = 0;
                    for (i = 0; i < n; s++, i++) s === r[0] && (s++, r.shift()), (a[i] = t[s]);
                    return a;
                }
                G.minProbeByteLength = 188;
                var X = G;
                function z(t, e) {
                    return (z =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var Q = (function (t) {
                    var e, r;
                    function i() {
                        return t.apply(this, arguments) || this;
                    }
                    (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), z(e, r);
                    var n = i.prototype;
                    return (
                        (n.resetInitSegment = function (e, r, i) {
                            t.prototype.resetInitSegment.call(this, e, r, i),
                                (this._audioTrack = { container: "audio/mpeg", type: "audio", id: 0, pid: -1, sequenceNumber: 0, isAAC: !1, samples: [], manifestCodec: e, duration: i, inputTimeScale: 9e4, dropped: 0 });
                        }),
                        (i.probe = function (t) {
                            if (!t) return !1;
                            for (var e = (o.b(t, 0) || []).length, r = t.length; e < r; e++) if (F(t, e)) return f.b.log("MPEG Audio sync word found !"), !0;
                            return !1;
                        }),
                        (n.canParse = function (t, e) {
                            return (function (t, e) {
                                return P(t, e) && 4 <= t.length - e;
                            })(t, e);
                        }),
                        (n.appendFrame = function (t, e, r) {
                            if (null !== this.initPTS) return O(t, e, r, this.initPTS, this.frameIndex);
                        }),
                        i
                    );
                })(c);
                Q.minProbeByteLength = 4;
                var $ = Q,
                    J = r(16),
                    Z = r(5),
                    tt = (function () {
                        function t() {
                            (this.emitInitSegment = !1), (this.audioCodec = void 0), (this.videoCodec = void 0), (this.initData = void 0), (this.initPTS = void 0), (this.initTracks = void 0), (this.lastEndDTS = null);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {}),
                            (e.resetTimeStamp = function (t) {
                                (this.initPTS = t), (this.lastEndDTS = null);
                            }),
                            (e.resetNextTimestamp = function () {
                                this.lastEndDTS = null;
                            }),
                            (e.resetInitSegment = function (t, e, r) {
                                (this.audioCodec = e), (this.videoCodec = r), this.generateInitSegment(t), (this.emitInitSegment = !0);
                            }),
                            (e.generateInitSegment = function (t) {
                                var e = this.audioCodec,
                                    r = this.videoCodec;
                                if (!t || !t.byteLength) return (this.initTracks = void 0), void (this.initData = void 0);
                                var i = (this.initData = Object(l.f)(t));
                                e || (e = rt(i.audio, Z.a.AUDIO)), r || (r = rt(i.video, Z.a.VIDEO));
                                var n = {};
                                i.audio && i.video
                                    ? (n.audiovideo = { container: "video/mp4", codec: e + "," + r, initSegment: t, id: "main" })
                                    : i.audio
                                    ? (n.audio = { container: "audio/mp4", codec: e, initSegment: t, id: "audio" })
                                    : i.video
                                    ? (n.video = { container: "video/mp4", codec: r, initSegment: t, id: "main" })
                                    : f.b.warn("[passthrough-remuxer.ts]: initSegment does not contain moov or trak boxes."),
                                    (this.initTracks = n);
                            }),
                            (e.remux = function (t, e, r, i, n) {
                                var a = this.initPTS,
                                    o = this.lastEndDTS,
                                    u = { audio: void 0, video: void 0, text: i, id3: r, initSegment: void 0 };
                                Object(s.a)(o) || (o = this.lastEndDTS = n || 0);
                                var h = e.samples;
                                if (!h || !h.length) return u;
                                var d = { initPTS: void 0, timescale: 1 },
                                    c = this.initData;
                                if (((c && c.length) || (this.generateInitSegment(h), (c = this.initData)), !c || !c.length)) return f.b.warn("[passthrough-remuxer.ts]: Failed to generate initSegment."), u;
                                this.emitInitSegment && ((d.tracks = this.initTracks), (this.emitInitSegment = !1)), Object(s.a)(a) || (this.initPTS = d.initPTS = a = et(c, h, o));
                                var g = Object(l.c)(h, c),
                                    v = o,
                                    p = g + v;
                                Object(l.e)(c, h, a), g > 0 ? (this.lastEndDTS = p) : (f.b.warn("Duration parsed from mp4 should be greater than zero"), this.resetNextTimestamp());
                                var m = !!c.audio,
                                    y = !!c.video,
                                    T = "";
                                m && (T += "audio"), y && (T += "video");
                                var b = { data1: h, startPTS: v, startDTS: v, endPTS: p, endDTS: p, type: T, hasAudio: m, hasVideo: y, nb: 1, dropped: 0 };
                                return (u.audio = "audio" === b.type ? b : void 0), (u.video = "audio" !== b.type ? b : void 0), (u.text = i), (u.id3 = r), (u.initSegment = d), u;
                            }),
                            t
                        );
                    })(),
                    et = function (t, e, r) {
                        return Object(l.d)(t, e) - r;
                    };
                function rt(t, e) {
                    var r = null == t ? void 0 : t.codec;
                    return r && r.length > 4 ? r : "hvc1" === r ? "hvc1.1.c.L120.90" : "av01" === r ? "av01.0.04M.08" : "avc1" === r || e === Z.a.VIDEO ? "avc1.42e01e" : "mp4a.40.5";
                }
                var it,
                    nt = tt,
                    at = r(13);
                try {
                    it = self.performance.now.bind(self.performance);
                } catch (t) {
                    f.b.debug("Unable to use Performance API on this environment"), (it = self.Date.now);
                }
                var st = [
                        { demux: X, remux: J.a },
                        { demux: D, remux: nt },
                        { demux: A, remux: J.a },
                        { demux: $, remux: J.a },
                    ],
                    ot = 1024;
                st.forEach(function (t) {
                    var e = t.demux;
                    ot = Math.max(ot, e.minProbeByteLength);
                });
                var lt = (function () {
                    function t(t, e, r, i, n) {
                        (this.observer = void 0),
                            (this.typeSupported = void 0),
                            (this.config = void 0),
                            (this.vendor = void 0),
                            (this.id = void 0),
                            (this.demuxer = void 0),
                            (this.remuxer = void 0),
                            (this.decrypter = void 0),
                            (this.probe = void 0),
                            (this.decryptionPromise = null),
                            (this.transmuxConfig = void 0),
                            (this.currentTransmuxState = void 0),
                            (this.cache = new at.a()),
                            (this.observer = t),
                            (this.typeSupported = e),
                            (this.config = r),
                            (this.vendor = i),
                            (this.id = n);
                    }
                    var e = t.prototype;
                    return (
                        (e.configure = function (t) {
                            (this.transmuxConfig = t), this.decrypter && this.decrypter.reset();
                        }),
                        (e.push = function (t, e, r, i) {
                            var n = this,
                                a = r.transmuxing;
                            a.executeStart = it();
                            var s = new Uint8Array(t),
                                o = this.cache,
                                u = this.config,
                                h = this.currentTransmuxState,
                                d = this.transmuxConfig;
                            i && (this.currentTransmuxState = i);
                            var c = (function (t, e) {
                                var r = null;
                                t.byteLength > 0 && null != e && null != e.key && null !== e.iv && null != e.method && (r = e);
                                return r;
                            })(s, e);
                            if (c && "AES-128" === c.method) {
                                var f = this.getDecrypter();
                                if (!u.enableSoftwareAES)
                                    return (
                                        (this.decryptionPromise = f.webCryptoDecrypt(s, c.key.buffer, c.iv.buffer).then(function (t) {
                                            var e = n.push(t, null, r);
                                            return (n.decryptionPromise = null), e;
                                        })),
                                        this.decryptionPromise
                                    );
                                var g = f.softwareDecrypt(s, c.key.buffer, c.iv.buffer);
                                if (!g) return (a.executeEnd = it()), ut(r);
                                s = new Uint8Array(g);
                            }
                            var v = i || h,
                                p = v.contiguous,
                                m = v.discontinuity,
                                y = v.trackSwitch,
                                T = v.accurateTimeOffset,
                                b = v.timeOffset,
                                E = d.audioCodec,
                                S = d.videoCodec,
                                L = d.defaultInitPts,
                                A = d.duration,
                                R = d.initSegmentData;
                            if (((m || y) && this.resetInitSegment(R, E, S, A), m && this.resetInitialTimestamp(L), p || this.resetContiguity(), this.needsProbing(s, m, y))) {
                                if (o.dataLength) {
                                    var D = o.flush();
                                    s = Object(l.a)(D, s);
                                }
                                this.configureTransmuxer(s, d);
                            }
                            var k = this.transmux(s, c, b, T, r),
                                _ = this.currentTransmuxState;
                            return (_.contiguous = !0), (_.discontinuity = !1), (_.trackSwitch = !1), (a.executeEnd = it()), k;
                        }),
                        (e.flush = function (t) {
                            var e = this,
                                r = t.transmuxing;
                            r.executeStart = it();
                            var a = this.decrypter,
                                s = this.cache,
                                o = this.currentTransmuxState,
                                l = this.decryptionPromise;
                            if (l)
                                return l.then(function () {
                                    return e.flush(t);
                                });
                            var u = [],
                                h = o.timeOffset;
                            if (a) {
                                var d = a.flush();
                                d && u.push(this.push(d, null, t));
                            }
                            var c = s.dataLength;
                            s.reset();
                            var f = this.demuxer,
                                g = this.remuxer;
                            if (!f || !g)
                                return (
                                    c >= ot && this.observer.emit(i.a.ERROR, i.a.ERROR, { type: n.b.MEDIA_ERROR, details: n.a.FRAG_PARSING_ERROR, fatal: !0, reason: "no demux matching with content found" }), (r.executeEnd = it()), [ut(t)]
                                );
                            var v = f.flush(h);
                            return ht(v)
                                ? v.then(function (r) {
                                      return e.flushRemux(u, r, t), u;
                                  })
                                : (this.flushRemux(u, v, t), u);
                        }),
                        (e.flushRemux = function (t, e, r) {
                            var i = e.audioTrack,
                                n = e.avcTrack,
                                a = e.id3Track,
                                s = e.textTrack,
                                o = this.currentTransmuxState,
                                l = o.accurateTimeOffset,
                                u = o.timeOffset;
                            f.b.log("[transmuxer.ts]: Flushed fragment " + r.sn + (r.part > -1 ? " p: " + r.part : "") + " of level " + r.level);
                            var h = this.remuxer.remux(i, n, a, s, u, l, !0, this.id);
                            t.push({ remuxResult: h, chunkMeta: r }), (r.transmuxing.executeEnd = it());
                        }),
                        (e.resetInitialTimestamp = function (t) {
                            var e = this.demuxer,
                                r = this.remuxer;
                            e && r && (e.resetTimeStamp(t), r.resetTimeStamp(t));
                        }),
                        (e.resetContiguity = function () {
                            var t = this.demuxer,
                                e = this.remuxer;
                            t && e && (t.resetContiguity(), e.resetNextTimestamp());
                        }),
                        (e.resetInitSegment = function (t, e, r, i) {
                            var n = this.demuxer,
                                a = this.remuxer;
                            n && a && (n.resetInitSegment(e, r, i), a.resetInitSegment(t, e, r));
                        }),
                        (e.destroy = function () {
                            this.demuxer && (this.demuxer.destroy(), (this.demuxer = void 0)), this.remuxer && (this.remuxer.destroy(), (this.remuxer = void 0));
                        }),
                        (e.transmux = function (t, e, r, i, n) {
                            return e && "SAMPLE-AES" === e.method ? this.transmuxSampleAes(t, e, r, i, n) : this.transmuxUnencrypted(t, r, i, n);
                        }),
                        (e.transmuxUnencrypted = function (t, e, r, i) {
                            var n = this.demuxer.demux(t, e, !1, !this.config.progressive),
                                a = n.audioTrack,
                                s = n.avcTrack,
                                o = n.id3Track,
                                l = n.textTrack;
                            return { remuxResult: this.remuxer.remux(a, s, o, l, e, r, !1, this.id), chunkMeta: i };
                        }),
                        (e.transmuxSampleAes = function (t, e, r, i, n) {
                            var a = this;
                            return this.demuxer.demuxSampleAes(t, e, r).then(function (t) {
                                return { remuxResult: a.remuxer.remux(t.audioTrack, t.avcTrack, t.id3Track, t.textTrack, r, i, !1, a.id), chunkMeta: n };
                            });
                        }),
                        (e.configureTransmuxer = function (t, e) {
                            for (
                                var r, i = this.config, n = this.observer, a = this.typeSupported, s = this.vendor, o = e.audioCodec, l = e.defaultInitPts, u = e.duration, h = e.initSegmentData, d = e.videoCodec, c = 0, g = st.length;
                                c < g;
                                c++
                            )
                                if (st[c].demux.probe(t)) {
                                    r = st[c];
                                    break;
                                }
                            r || (f.b.warn("Failed to find demuxer by probing frag, treating as mp4 passthrough"), (r = { demux: D, remux: nt }));
                            var v = this.demuxer,
                                p = this.remuxer,
                                m = r.remux,
                                y = r.demux;
                            (p && p instanceof m) || (this.remuxer = new m(n, i, a, s)), (v && v instanceof y) || ((this.demuxer = new y(n, i, a)), (this.probe = y.probe)), this.resetInitSegment(h, o, d, u), this.resetInitialTimestamp(l);
                        }),
                        (e.needsProbing = function (t, e, r) {
                            return !this.demuxer || !this.remuxer || e || r;
                        }),
                        (e.getDecrypter = function () {
                            var t = this.decrypter;
                            return t || (t = this.decrypter = new a.a(this.observer, this.config)), t;
                        }),
                        t
                    );
                })();
                var ut = function (t) {
                    return { remuxResult: {}, chunkMeta: t };
                };
                function ht(t) {
                    return "then" in t && t.then instanceof Function;
                }
                var dt = function (t, e, r, i, n) {
                        (this.audioCodec = void 0),
                            (this.videoCodec = void 0),
                            (this.initSegmentData = void 0),
                            (this.duration = void 0),
                            (this.defaultInitPts = void 0),
                            (this.audioCodec = t),
                            (this.videoCodec = e),
                            (this.initSegmentData = r),
                            (this.duration = i),
                            (this.defaultInitPts = n);
                    },
                    ct = function (t, e, r, i, n) {
                        (this.discontinuity = void 0),
                            (this.contiguous = void 0),
                            (this.accurateTimeOffset = void 0),
                            (this.trackSwitch = void 0),
                            (this.timeOffset = void 0),
                            (this.discontinuity = t),
                            (this.contiguous = e),
                            (this.accurateTimeOffset = r),
                            (this.trackSwitch = i),
                            (this.timeOffset = n);
                    };
            },
            function (t, e, r) {
                var i, n, a, s, o;
                (i = /^((?:[a-zA-Z0-9+\-.]+:)?)(\/\/[^\/?#]*)?((?:[^\/?#]*\/)*[^;?#]*)?(;[^?#]*)?(\?[^#]*)?(#[^]*)?$/),
                    (n = /^([^\/?#]*)([^]*)$/),
                    (a = /(?:\/|^)\.(?=\/)/g),
                    (s = /(?:\/|^)\.\.\/(?!\.\.\/)[^\/]*(?=\/)/g),
                    (o = {
                        buildAbsoluteURL: function (t, e, r) {
                            if (((r = r || {}), (t = t.trim()), !(e = e.trim()))) {
                                if (!r.alwaysNormalize) return t;
                                var i = o.parseURL(t);
                                if (!i) throw new Error("Error trying to parse base URL.");
                                return (i.path = o.normalizePath(i.path)), o.buildURLFromParts(i);
                            }
                            var a = o.parseURL(e);
                            if (!a) throw new Error("Error trying to parse relative URL.");
                            if (a.scheme) return r.alwaysNormalize ? ((a.path = o.normalizePath(a.path)), o.buildURLFromParts(a)) : e;
                            var s = o.parseURL(t);
                            if (!s) throw new Error("Error trying to parse base URL.");
                            if (!s.netLoc && s.path && "/" !== s.path[0]) {
                                var l = n.exec(s.path);
                                (s.netLoc = l[1]), (s.path = l[2]);
                            }
                            s.netLoc && !s.path && (s.path = "/");
                            var u = { scheme: s.scheme, netLoc: a.netLoc, path: null, params: a.params, query: a.query, fragment: a.fragment };
                            if (!a.netLoc && ((u.netLoc = s.netLoc), "/" !== a.path[0]))
                                if (a.path) {
                                    var h = s.path,
                                        d = h.substring(0, h.lastIndexOf("/") + 1) + a.path;
                                    u.path = o.normalizePath(d);
                                } else (u.path = s.path), a.params || ((u.params = s.params), a.query || (u.query = s.query));
                            return null === u.path && (u.path = r.alwaysNormalize ? o.normalizePath(a.path) : a.path), o.buildURLFromParts(u);
                        },
                        parseURL: function (t) {
                            var e = i.exec(t);
                            return e ? { scheme: e[1] || "", netLoc: e[2] || "", path: e[3] || "", params: e[4] || "", query: e[5] || "", fragment: e[6] || "" } : null;
                        },
                        normalizePath: function (t) {
                            for (t = t.split("").reverse().join("").replace(a, ""); t.length !== (t = t.replace(s, "")).length; );
                            return t.split("").reverse().join("");
                        },
                        buildURLFromParts: function (t) {
                            return t.scheme + t.netLoc + t.path + t.params + t.query + t.fragment;
                        },
                    }),
                    (t.exports = o);
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return i;
                });
                var i = function () {
                    (this.aborted = !1),
                        (this.loaded = 0),
                        (this.retry = 0),
                        (this.total = 0),
                        (this.chunkCount = 0),
                        (this.bwEstimate = 0),
                        (this.loading = { start: 0, first: 0, end: 0 }),
                        (this.parsing = { start: 0, end: 0 }),
                        (this.buffering = { start: 0, first: 0, end: 0 });
                };
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return i;
                });
                var i = (function () {
                    function t() {
                        (this.chunks = []), (this.dataLength = 0);
                    }
                    var e = t.prototype;
                    return (
                        (e.push = function (t) {
                            this.chunks.push(t), (this.dataLength += t.length);
                        }),
                        (e.flush = function () {
                            var t,
                                e = this.chunks,
                                r = this.dataLength;
                            return e.length
                                ? ((t =
                                      1 === e.length
                                          ? e[0]
                                          : (function (t, e) {
                                                for (var r = new Uint8Array(e), i = 0, n = 0; n < t.length; n++) {
                                                    var a = t[n];
                                                    r.set(a, i), (i += a.length);
                                                }
                                                return r;
                                            })(e, r)),
                                  this.reset(),
                                  t)
                                : new Uint8Array(0);
                        }),
                        (e.reset = function () {
                            (this.chunks.length = 0), (this.dataLength = 0);
                        }),
                        t
                    );
                })();
            },
            function (t, e, r) {
                "use strict";
                var i = Object.prototype.hasOwnProperty,
                    n = "~";
                function a() {}
                function s(t, e, r) {
                    (this.fn = t), (this.context = e), (this.once = r || !1);
                }
                function o(t, e, r, i, a) {
                    if ("function" != typeof r) throw new TypeError("The listener must be a function");
                    var o = new s(r, i || t, a),
                        l = n ? n + e : e;
                    return t._events[l] ? (t._events[l].fn ? (t._events[l] = [t._events[l], o]) : t._events[l].push(o)) : ((t._events[l] = o), t._eventsCount++), t;
                }
                function l(t, e) {
                    0 == --t._eventsCount ? (t._events = new a()) : delete t._events[e];
                }
                function u() {
                    (this._events = new a()), (this._eventsCount = 0);
                }
                Object.create && ((a.prototype = Object.create(null)), new a().__proto__ || (n = !1)),
                    (u.prototype.eventNames = function () {
                        var t,
                            e,
                            r = [];
                        if (0 === this._eventsCount) return r;
                        for (e in (t = this._events)) i.call(t, e) && r.push(n ? e.slice(1) : e);
                        return Object.getOwnPropertySymbols ? r.concat(Object.getOwnPropertySymbols(t)) : r;
                    }),
                    (u.prototype.listeners = function (t) {
                        var e = n ? n + t : t,
                            r = this._events[e];
                        if (!r) return [];
                        if (r.fn) return [r.fn];
                        for (var i = 0, a = r.length, s = new Array(a); i < a; i++) s[i] = r[i].fn;
                        return s;
                    }),
                    (u.prototype.listenerCount = function (t) {
                        var e = n ? n + t : t,
                            r = this._events[e];
                        return r ? (r.fn ? 1 : r.length) : 0;
                    }),
                    (u.prototype.emit = function (t, e, r, i, a, s) {
                        var o = n ? n + t : t;
                        if (!this._events[o]) return !1;
                        var l,
                            u,
                            h = this._events[o],
                            d = arguments.length;
                        if (h.fn) {
                            switch ((h.once && this.removeListener(t, h.fn, void 0, !0), d)) {
                                case 1:
                                    return h.fn.call(h.context), !0;
                                case 2:
                                    return h.fn.call(h.context, e), !0;
                                case 3:
                                    return h.fn.call(h.context, e, r), !0;
                                case 4:
                                    return h.fn.call(h.context, e, r, i), !0;
                                case 5:
                                    return h.fn.call(h.context, e, r, i, a), !0;
                                case 6:
                                    return h.fn.call(h.context, e, r, i, a, s), !0;
                            }
                            for (u = 1, l = new Array(d - 1); u < d; u++) l[u - 1] = arguments[u];
                            h.fn.apply(h.context, l);
                        } else {
                            var c,
                                f = h.length;
                            for (u = 0; u < f; u++)
                                switch ((h[u].once && this.removeListener(t, h[u].fn, void 0, !0), d)) {
                                    case 1:
                                        h[u].fn.call(h[u].context);
                                        break;
                                    case 2:
                                        h[u].fn.call(h[u].context, e);
                                        break;
                                    case 3:
                                        h[u].fn.call(h[u].context, e, r);
                                        break;
                                    case 4:
                                        h[u].fn.call(h[u].context, e, r, i);
                                        break;
                                    default:
                                        if (!l) for (c = 1, l = new Array(d - 1); c < d; c++) l[c - 1] = arguments[c];
                                        h[u].fn.apply(h[u].context, l);
                                }
                        }
                        return !0;
                    }),
                    (u.prototype.on = function (t, e, r) {
                        return o(this, t, e, r, !1);
                    }),
                    (u.prototype.once = function (t, e, r) {
                        return o(this, t, e, r, !0);
                    }),
                    (u.prototype.removeListener = function (t, e, r, i) {
                        var a = n ? n + t : t;
                        if (!this._events[a]) return this;
                        if (!e) return l(this, a), this;
                        var s = this._events[a];
                        if (s.fn) s.fn !== e || (i && !s.once) || (r && s.context !== r) || l(this, a);
                        else {
                            for (var o = 0, u = [], h = s.length; o < h; o++) (s[o].fn !== e || (i && !s[o].once) || (r && s[o].context !== r)) && u.push(s[o]);
                            u.length ? (this._events[a] = 1 === u.length ? u[0] : u) : l(this, a);
                        }
                        return this;
                    }),
                    (u.prototype.removeAllListeners = function (t) {
                        var e;
                        return t ? ((e = n ? n + t : t), this._events[e] && l(this, e)) : ((this._events = new a()), (this._eventsCount = 0)), this;
                    }),
                    (u.prototype.off = u.prototype.removeListener),
                    (u.prototype.addListener = u.prototype.on),
                    (u.prefixed = n),
                    (u.EventEmitter = u),
                    (t.exports = u);
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return u;
                });
                var i = (function () {
                        function t(t, e) {
                            (this.subtle = void 0), (this.aesIV = void 0), (this.subtle = t), (this.aesIV = e);
                        }
                        return (
                            (t.prototype.decrypt = function (t, e) {
                                return this.subtle.decrypt({ name: "AES-CBC", iv: this.aesIV }, e, t);
                            }),
                            t
                        );
                    })(),
                    n = (function () {
                        function t(t, e) {
                            (this.subtle = void 0), (this.key = void 0), (this.subtle = t), (this.key = e);
                        }
                        return (
                            (t.prototype.expandKey = function () {
                                return this.subtle.importKey("raw", this.key, { name: "AES-CBC" }, !1, ["encrypt", "decrypt"]);
                            }),
                            t
                        );
                    })(),
                    a = r(9);
                var s = (function () {
                        function t() {
                            (this.rcon = [0, 1, 2, 4, 8, 16, 32, 64, 128, 27, 54]),
                                (this.subMix = [new Uint32Array(256), new Uint32Array(256), new Uint32Array(256), new Uint32Array(256)]),
                                (this.invSubMix = [new Uint32Array(256), new Uint32Array(256), new Uint32Array(256), new Uint32Array(256)]),
                                (this.sBox = new Uint32Array(256)),
                                (this.invSBox = new Uint32Array(256)),
                                (this.key = new Uint32Array(0)),
                                (this.ksRows = 0),
                                (this.keySize = 0),
                                (this.keySchedule = void 0),
                                (this.invKeySchedule = void 0),
                                this.initTable();
                        }
                        var e = t.prototype;
                        return (
                            (e.uint8ArrayToUint32Array_ = function (t) {
                                for (var e = new DataView(t), r = new Uint32Array(4), i = 0; i < 4; i++) r[i] = e.getUint32(4 * i);
                                return r;
                            }),
                            (e.initTable = function () {
                                var t = this.sBox,
                                    e = this.invSBox,
                                    r = this.subMix,
                                    i = r[0],
                                    n = r[1],
                                    a = r[2],
                                    s = r[3],
                                    o = this.invSubMix,
                                    l = o[0],
                                    u = o[1],
                                    h = o[2],
                                    d = o[3],
                                    c = new Uint32Array(256),
                                    f = 0,
                                    g = 0,
                                    v = 0;
                                for (v = 0; v < 256; v++) c[v] = v < 128 ? v << 1 : (v << 1) ^ 283;
                                for (v = 0; v < 256; v++) {
                                    var p = g ^ (g << 1) ^ (g << 2) ^ (g << 3) ^ (g << 4);
                                    (p = (p >>> 8) ^ (255 & p) ^ 99), (t[f] = p), (e[p] = f);
                                    var m = c[f],
                                        y = c[m],
                                        T = c[y],
                                        b = (257 * c[p]) ^ (16843008 * p);
                                    (i[f] = (b << 24) | (b >>> 8)),
                                        (n[f] = (b << 16) | (b >>> 16)),
                                        (a[f] = (b << 8) | (b >>> 24)),
                                        (s[f] = b),
                                        (b = (16843009 * T) ^ (65537 * y) ^ (257 * m) ^ (16843008 * f)),
                                        (l[p] = (b << 24) | (b >>> 8)),
                                        (u[p] = (b << 16) | (b >>> 16)),
                                        (h[p] = (b << 8) | (b >>> 24)),
                                        (d[p] = b),
                                        f ? ((f = m ^ c[c[c[T ^ m]]]), (g ^= c[c[g]])) : (f = g = 1);
                                }
                            }),
                            (e.expandKey = function (t) {
                                for (var e = this.uint8ArrayToUint32Array_(t), r = !0, i = 0; i < e.length && r; ) (r = e[i] === this.key[i]), i++;
                                if (!r) {
                                    this.key = e;
                                    var n = (this.keySize = e.length);
                                    if (4 !== n && 6 !== n && 8 !== n) throw new Error("Invalid aes key size=" + n);
                                    var a,
                                        s,
                                        o,
                                        l,
                                        u = (this.ksRows = 4 * (n + 6 + 1)),
                                        h = (this.keySchedule = new Uint32Array(u)),
                                        d = (this.invKeySchedule = new Uint32Array(u)),
                                        c = this.sBox,
                                        f = this.rcon,
                                        g = this.invSubMix,
                                        v = g[0],
                                        p = g[1],
                                        m = g[2],
                                        y = g[3];
                                    for (a = 0; a < u; a++)
                                        a < n
                                            ? (o = h[a] = e[a])
                                            : ((l = o),
                                              a % n == 0
                                                  ? ((l = (c[(l = (l << 8) | (l >>> 24)) >>> 24] << 24) | (c[(l >>> 16) & 255] << 16) | (c[(l >>> 8) & 255] << 8) | c[255 & l]), (l ^= f[(a / n) | 0] << 24))
                                                  : n > 6 && a % n == 4 && (l = (c[l >>> 24] << 24) | (c[(l >>> 16) & 255] << 16) | (c[(l >>> 8) & 255] << 8) | c[255 & l]),
                                              (h[a] = o = (h[a - n] ^ l) >>> 0));
                                    for (s = 0; s < u; s++) (a = u - s), (l = 3 & s ? h[a] : h[a - 4]), (d[s] = s < 4 || a <= 4 ? l : v[c[l >>> 24]] ^ p[c[(l >>> 16) & 255]] ^ m[c[(l >>> 8) & 255]] ^ y[c[255 & l]]), (d[s] = d[s] >>> 0);
                                }
                            }),
                            (e.networkToHostOrderSwap = function (t) {
                                return (t << 24) | ((65280 & t) << 8) | ((16711680 & t) >> 8) | (t >>> 24);
                            }),
                            (e.decrypt = function (t, e, r) {
                                for (
                                    var i,
                                        n,
                                        a,
                                        s,
                                        o,
                                        l,
                                        u,
                                        h,
                                        d,
                                        c,
                                        f,
                                        g,
                                        v,
                                        p,
                                        m = this.keySize + 6,
                                        y = this.invKeySchedule,
                                        T = this.invSBox,
                                        b = this.invSubMix,
                                        E = b[0],
                                        S = b[1],
                                        L = b[2],
                                        A = b[3],
                                        R = this.uint8ArrayToUint32Array_(r),
                                        D = R[0],
                                        k = R[1],
                                        _ = R[2],
                                        I = R[3],
                                        C = new Int32Array(t),
                                        w = new Int32Array(C.length),
                                        O = this.networkToHostOrderSwap;
                                    e < C.length;

                                ) {
                                    for (d = O(C[e]), c = O(C[e + 1]), f = O(C[e + 2]), g = O(C[e + 3]), o = d ^ y[0], l = g ^ y[1], u = f ^ y[2], h = c ^ y[3], v = 4, p = 1; p < m; p++)
                                        (i = E[o >>> 24] ^ S[(l >> 16) & 255] ^ L[(u >> 8) & 255] ^ A[255 & h] ^ y[v]),
                                            (n = E[l >>> 24] ^ S[(u >> 16) & 255] ^ L[(h >> 8) & 255] ^ A[255 & o] ^ y[v + 1]),
                                            (a = E[u >>> 24] ^ S[(h >> 16) & 255] ^ L[(o >> 8) & 255] ^ A[255 & l] ^ y[v + 2]),
                                            (s = E[h >>> 24] ^ S[(o >> 16) & 255] ^ L[(l >> 8) & 255] ^ A[255 & u] ^ y[v + 3]),
                                            (o = i),
                                            (l = n),
                                            (u = a),
                                            (h = s),
                                            (v += 4);
                                    (i = (T[o >>> 24] << 24) ^ (T[(l >> 16) & 255] << 16) ^ (T[(u >> 8) & 255] << 8) ^ T[255 & h] ^ y[v]),
                                        (n = (T[l >>> 24] << 24) ^ (T[(u >> 16) & 255] << 16) ^ (T[(h >> 8) & 255] << 8) ^ T[255 & o] ^ y[v + 1]),
                                        (a = (T[u >>> 24] << 24) ^ (T[(h >> 16) & 255] << 16) ^ (T[(o >> 8) & 255] << 8) ^ T[255 & l] ^ y[v + 2]),
                                        (s = (T[h >>> 24] << 24) ^ (T[(o >> 16) & 255] << 16) ^ (T[(l >> 8) & 255] << 8) ^ T[255 & u] ^ y[v + 3]),
                                        (w[e] = O(i ^ D)),
                                        (w[e + 1] = O(s ^ k)),
                                        (w[e + 2] = O(a ^ _)),
                                        (w[e + 3] = O(n ^ I)),
                                        (D = d),
                                        (k = c),
                                        (_ = f),
                                        (I = g),
                                        (e += 4);
                                }
                                return w.buffer;
                            }),
                            t
                        );
                    })(),
                    o = r(1),
                    l = r(6),
                    u = (function () {
                        function t(t, e, r) {
                            var i = (void 0 === r ? {} : r).removePKCS7Padding,
                                n = void 0 === i || i;
                            if (
                                ((this.logEnabled = !0),
                                (this.observer = void 0),
                                (this.config = void 0),
                                (this.removePKCS7Padding = void 0),
                                (this.subtle = null),
                                (this.softwareDecrypter = null),
                                (this.key = null),
                                (this.fastAesKey = null),
                                (this.remainderData = null),
                                (this.currentIV = null),
                                (this.currentResult = null),
                                (this.observer = t),
                                (this.config = e),
                                (this.removePKCS7Padding = n),
                                n)
                            )
                                try {
                                    var a = self.crypto;
                                    a && (this.subtle = a.subtle || a.webkitSubtle);
                                } catch (t) {}
                            null === this.subtle && (this.config.enableSoftwareAES = !0);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                this.observer = null;
                            }),
                            (e.isSync = function () {
                                return this.config.enableSoftwareAES;
                            }),
                            (e.flush = function () {
                                var t = this.currentResult;
                                if (t) {
                                    var e,
                                        r,
                                        i,
                                        n = new Uint8Array(t);
                                    return this.reset(), this.removePKCS7Padding ? ((r = (e = n).byteLength), (i = r && new DataView(e.buffer).getUint8(r - 1)) ? Object(a.a)(e, 0, r - i) : e) : n;
                                }
                                this.reset();
                            }),
                            (e.reset = function () {
                                (this.currentResult = null), (this.currentIV = null), (this.remainderData = null), this.softwareDecrypter && (this.softwareDecrypter = null);
                            }),
                            (e.decrypt = function (t, e, r, i) {
                                if (this.config.enableSoftwareAES) {
                                    this.softwareDecrypt(new Uint8Array(t), e, r);
                                    var n = this.flush();
                                    n && i(n.buffer);
                                } else this.webCryptoDecrypt(new Uint8Array(t), e, r).then(i);
                            }),
                            (e.softwareDecrypt = function (t, e, r) {
                                var i = this.currentIV,
                                    n = this.currentResult,
                                    o = this.remainderData;
                                this.logOnce("JS AES decrypt"), o && ((t = Object(l.a)(o, t)), (this.remainderData = null));
                                var u = this.getValidChunk(t);
                                if (!u.length) return null;
                                i && (r = i);
                                var h = this.softwareDecrypter;
                                h || (h = this.softwareDecrypter = new s()), h.expandKey(e);
                                var d = n;
                                return (this.currentResult = h.decrypt(u.buffer, 0, r)), (this.currentIV = Object(a.a)(u, -16).buffer), d || null;
                            }),
                            (e.webCryptoDecrypt = function (t, e, r) {
                                var a = this,
                                    s = this.subtle;
                                return (
                                    (this.key === e && this.fastAesKey) || ((this.key = e), (this.fastAesKey = new n(s, e))),
                                    this.fastAesKey
                                        .expandKey()
                                        .then(function (e) {
                                            return s ? new i(s, r).decrypt(t.buffer, e) : Promise.reject(new Error("web crypto not initialized"));
                                        })
                                        .catch(function (i) {
                                            return a.onWebCryptoError(i, t, e, r);
                                        })
                                );
                            }),
                            (e.onWebCryptoError = function (t, e, r, i) {
                                return o.b.warn("[decrypter.ts]: WebCrypto Error, disable WebCrypto API:", t), (this.config.enableSoftwareAES = !0), (this.logEnabled = !0), this.softwareDecrypt(e, r, i);
                            }),
                            (e.getValidChunk = function (t) {
                                var e = t,
                                    r = t.length - (t.length % 16);
                                return r !== t.length && ((e = Object(a.a)(t, 0, r)), (this.remainderData = Object(a.a)(t, r))), e;
                            }),
                            (e.logOnce = function (t) {
                                this.logEnabled && (o.b.log("[decrypter.ts]: " + t), (this.logEnabled = !1));
                            }),
                            t
                        );
                    })();
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return m;
                }),
                    r.d(e, "b", function () {
                        return y;
                    });
                var i = r(3),
                    n = (function () {
                        function t() {}
                        return (
                            (t.getSilentFrame = function (t, e) {
                                switch (t) {
                                    case "mp4a.40.2":
                                        if (1 === e) return new Uint8Array([0, 200, 0, 128, 35, 128]);
                                        if (2 === e) return new Uint8Array([33, 0, 73, 144, 2, 25, 0, 35, 128]);
                                        if (3 === e) return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 142]);
                                        if (4 === e) return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 128, 44, 128, 8, 2, 56]);
                                        if (5 === e) return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 130, 48, 4, 153, 0, 33, 144, 2, 56]);
                                        if (6 === e) return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 130, 48, 4, 153, 0, 33, 144, 2, 0, 178, 0, 32, 8, 224]);
                                        break;
                                    default:
                                        if (1 === e)
                                            return new Uint8Array([
                                                1,
                                                64,
                                                34,
                                                128,
                                                163,
                                                78,
                                                230,
                                                128,
                                                186,
                                                8,
                                                0,
                                                0,
                                                0,
                                                28,
                                                6,
                                                241,
                                                193,
                                                10,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                94,
                                            ]);
                                        if (2 === e)
                                            return new Uint8Array([
                                                1,
                                                64,
                                                34,
                                                128,
                                                163,
                                                94,
                                                230,
                                                128,
                                                186,
                                                8,
                                                0,
                                                0,
                                                0,
                                                0,
                                                149,
                                                0,
                                                6,
                                                241,
                                                161,
                                                10,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                94,
                                            ]);
                                        if (3 === e)
                                            return new Uint8Array([
                                                1,
                                                64,
                                                34,
                                                128,
                                                163,
                                                94,
                                                230,
                                                128,
                                                186,
                                                8,
                                                0,
                                                0,
                                                0,
                                                0,
                                                149,
                                                0,
                                                6,
                                                241,
                                                161,
                                                10,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                90,
                                                94,
                                            ]);
                                }
                            }),
                            t
                        );
                    })(),
                    a = Math.pow(2, 32) - 1,
                    s = (function () {
                        function t() {}
                        return (
                            (t.init = function () {
                                var e;
                                for (e in ((t.types = {
                                    avc1: [],
                                    avcC: [],
                                    btrt: [],
                                    dinf: [],
                                    dref: [],
                                    esds: [],
                                    ftyp: [],
                                    hdlr: [],
                                    mdat: [],
                                    mdhd: [],
                                    mdia: [],
                                    mfhd: [],
                                    minf: [],
                                    moof: [],
                                    moov: [],
                                    mp4a: [],
                                    ".mp3": [],
                                    mvex: [],
                                    mvhd: [],
                                    pasp: [],
                                    sdtp: [],
                                    stbl: [],
                                    stco: [],
                                    stsc: [],
                                    stsd: [],
                                    stsz: [],
                                    stts: [],
                                    tfdt: [],
                                    tfhd: [],
                                    traf: [],
                                    trak: [],
                                    trun: [],
                                    trex: [],
                                    tkhd: [],
                                    vmhd: [],
                                    smhd: [],
                                }),
                                t.types))
                                    t.types.hasOwnProperty(e) && (t.types[e] = [e.charCodeAt(0), e.charCodeAt(1), e.charCodeAt(2), e.charCodeAt(3)]);
                                var r = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 118, 105, 100, 101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86, 105, 100, 101, 111, 72, 97, 110, 100, 108, 101, 114, 0]),
                                    i = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 115, 111, 117, 110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 83, 111, 117, 110, 100, 72, 97, 110, 100, 108, 101, 114, 0]);
                                t.HDLR_TYPES = { video: r, audio: i };
                                var n = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 12, 117, 114, 108, 32, 0, 0, 0, 1]),
                                    a = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0]);
                                (t.STTS = t.STSC = t.STCO = a),
                                    (t.STSZ = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0])),
                                    (t.VMHD = new Uint8Array([0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0])),
                                    (t.SMHD = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0])),
                                    (t.STSD = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1]));
                                var s = new Uint8Array([105, 115, 111, 109]),
                                    o = new Uint8Array([97, 118, 99, 49]),
                                    l = new Uint8Array([0, 0, 0, 1]);
                                (t.FTYP = t.box(t.types.ftyp, s, l, s, o)), (t.DINF = t.box(t.types.dinf, t.box(t.types.dref, n)));
                            }),
                            (t.box = function (t) {
                                for (var e = 8, r = arguments.length, i = new Array(r > 1 ? r - 1 : 0), n = 1; n < r; n++) i[n - 1] = arguments[n];
                                for (var a = i.length, s = a; a--; ) e += i[a].byteLength;
                                var o = new Uint8Array(e);
                                for (o[0] = (e >> 24) & 255, o[1] = (e >> 16) & 255, o[2] = (e >> 8) & 255, o[3] = 255 & e, o.set(t, 4), a = 0, e = 8; a < s; a++) o.set(i[a], e), (e += i[a].byteLength);
                                return o;
                            }),
                            (t.hdlr = function (e) {
                                return t.box(t.types.hdlr, t.HDLR_TYPES[e]);
                            }),
                            (t.mdat = function (e) {
                                return t.box(t.types.mdat, e);
                            }),
                            (t.mdhd = function (e, r) {
                                r *= e;
                                var i = Math.floor(r / (a + 1)),
                                    n = Math.floor(r % (a + 1));
                                return t.box(
                                    t.types.mdhd,
                                    new Uint8Array([
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        2,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        3,
                                        (e >> 24) & 255,
                                        (e >> 16) & 255,
                                        (e >> 8) & 255,
                                        255 & e,
                                        i >> 24,
                                        (i >> 16) & 255,
                                        (i >> 8) & 255,
                                        255 & i,
                                        n >> 24,
                                        (n >> 16) & 255,
                                        (n >> 8) & 255,
                                        255 & n,
                                        85,
                                        196,
                                        0,
                                        0,
                                    ])
                                );
                            }),
                            (t.mdia = function (e) {
                                return t.box(t.types.mdia, t.mdhd(e.timescale, e.duration), t.hdlr(e.type), t.minf(e));
                            }),
                            (t.mfhd = function (e) {
                                return t.box(t.types.mfhd, new Uint8Array([0, 0, 0, 0, e >> 24, (e >> 16) & 255, (e >> 8) & 255, 255 & e]));
                            }),
                            (t.minf = function (e) {
                                return "audio" === e.type ? t.box(t.types.minf, t.box(t.types.smhd, t.SMHD), t.DINF, t.stbl(e)) : t.box(t.types.minf, t.box(t.types.vmhd, t.VMHD), t.DINF, t.stbl(e));
                            }),
                            (t.moof = function (e, r, i) {
                                return t.box(t.types.moof, t.mfhd(e), t.traf(i, r));
                            }),
                            (t.moov = function (e) {
                                for (var r = e.length, i = []; r--; ) i[r] = t.trak(e[r]);
                                return t.box.apply(null, [t.types.moov, t.mvhd(e[0].timescale, e[0].duration)].concat(i).concat(t.mvex(e)));
                            }),
                            (t.mvex = function (e) {
                                for (var r = e.length, i = []; r--; ) i[r] = t.trex(e[r]);
                                return t.box.apply(null, [t.types.mvex].concat(i));
                            }),
                            (t.mvhd = function (e, r) {
                                r *= e;
                                var i = Math.floor(r / (a + 1)),
                                    n = Math.floor(r % (a + 1)),
                                    s = new Uint8Array([
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        2,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        3,
                                        (e >> 24) & 255,
                                        (e >> 16) & 255,
                                        (e >> 8) & 255,
                                        255 & e,
                                        i >> 24,
                                        (i >> 16) & 255,
                                        (i >> 8) & 255,
                                        255 & i,
                                        n >> 24,
                                        (n >> 16) & 255,
                                        (n >> 8) & 255,
                                        255 & n,
                                        0,
                                        1,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        64,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        255,
                                        255,
                                        255,
                                        255,
                                    ]);
                                return t.box(t.types.mvhd, s);
                            }),
                            (t.sdtp = function (e) {
                                var r,
                                    i,
                                    n = e.samples || [],
                                    a = new Uint8Array(4 + n.length);
                                for (r = 0; r < n.length; r++) (i = n[r].flags), (a[r + 4] = (i.dependsOn << 4) | (i.isDependedOn << 2) | i.hasRedundancy);
                                return t.box(t.types.sdtp, a);
                            }),
                            (t.stbl = function (e) {
                                return t.box(t.types.stbl, t.stsd(e), t.box(t.types.stts, t.STTS), t.box(t.types.stsc, t.STSC), t.box(t.types.stsz, t.STSZ), t.box(t.types.stco, t.STCO));
                            }),
                            (t.avc1 = function (e) {
                                var r,
                                    i,
                                    n,
                                    a = [],
                                    s = [];
                                for (r = 0; r < e.sps.length; r++) (n = (i = e.sps[r]).byteLength), a.push((n >>> 8) & 255), a.push(255 & n), (a = a.concat(Array.prototype.slice.call(i)));
                                for (r = 0; r < e.pps.length; r++) (n = (i = e.pps[r]).byteLength), s.push((n >>> 8) & 255), s.push(255 & n), (s = s.concat(Array.prototype.slice.call(i)));
                                var o = t.box(t.types.avcC, new Uint8Array([1, a[3], a[4], a[5], 255, 224 | e.sps.length].concat(a).concat([e.pps.length]).concat(s))),
                                    l = e.width,
                                    u = e.height,
                                    h = e.pixelRatio[0],
                                    d = e.pixelRatio[1];
                                return t.box(
                                    t.types.avc1,
                                    new Uint8Array([
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        (l >> 8) & 255,
                                        255 & l,
                                        (u >> 8) & 255,
                                        255 & u,
                                        0,
                                        72,
                                        0,
                                        0,
                                        0,
                                        72,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        18,
                                        100,
                                        97,
                                        105,
                                        108,
                                        121,
                                        109,
                                        111,
                                        116,
                                        105,
                                        111,
                                        110,
                                        47,
                                        104,
                                        108,
                                        115,
                                        46,
                                        106,
                                        115,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        24,
                                        17,
                                        17,
                                    ]),
                                    o,
                                    t.box(t.types.btrt, new Uint8Array([0, 28, 156, 128, 0, 45, 198, 192, 0, 45, 198, 192])),
                                    t.box(t.types.pasp, new Uint8Array([h >> 24, (h >> 16) & 255, (h >> 8) & 255, 255 & h, d >> 24, (d >> 16) & 255, (d >> 8) & 255, 255 & d]))
                                );
                            }),
                            (t.esds = function (t) {
                                var e = t.config.length;
                                return new Uint8Array([0, 0, 0, 0, 3, 23 + e, 0, 1, 0, 4, 15 + e, 64, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5].concat([e]).concat(t.config).concat([6, 1, 2]));
                            }),
                            (t.mp4a = function (e) {
                                var r = e.samplerate;
                                return t.box(t.types.mp4a, new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, e.channelCount, 0, 16, 0, 0, 0, 0, (r >> 8) & 255, 255 & r, 0, 0]), t.box(t.types.esds, t.esds(e)));
                            }),
                            (t.mp3 = function (e) {
                                var r = e.samplerate;
                                return t.box(t.types[".mp3"], new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, e.channelCount, 0, 16, 0, 0, 0, 0, (r >> 8) & 255, 255 & r, 0, 0]));
                            }),
                            (t.stsd = function (e) {
                                return "audio" === e.type ? (e.isAAC || "mp3" !== e.codec ? t.box(t.types.stsd, t.STSD, t.mp4a(e)) : t.box(t.types.stsd, t.STSD, t.mp3(e))) : t.box(t.types.stsd, t.STSD, t.avc1(e));
                            }),
                            (t.tkhd = function (e) {
                                var r = e.id,
                                    i = e.duration * e.timescale,
                                    n = e.width,
                                    s = e.height,
                                    o = Math.floor(i / (a + 1)),
                                    l = Math.floor(i % (a + 1));
                                return t.box(
                                    t.types.tkhd,
                                    new Uint8Array([
                                        1,
                                        0,
                                        0,
                                        7,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        2,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        3,
                                        (r >> 24) & 255,
                                        (r >> 16) & 255,
                                        (r >> 8) & 255,
                                        255 & r,
                                        0,
                                        0,
                                        0,
                                        0,
                                        o >> 24,
                                        (o >> 16) & 255,
                                        (o >> 8) & 255,
                                        255 & o,
                                        l >> 24,
                                        (l >> 16) & 255,
                                        (l >> 8) & 255,
                                        255 & l,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        64,
                                        0,
                                        0,
                                        0,
                                        (n >> 8) & 255,
                                        255 & n,
                                        0,
                                        0,
                                        (s >> 8) & 255,
                                        255 & s,
                                        0,
                                        0,
                                    ])
                                );
                            }),
                            (t.traf = function (e, r) {
                                var i = t.sdtp(e),
                                    n = e.id,
                                    s = Math.floor(r / (a + 1)),
                                    o = Math.floor(r % (a + 1));
                                return t.box(
                                    t.types.traf,
                                    t.box(t.types.tfhd, new Uint8Array([0, 0, 0, 0, n >> 24, (n >> 16) & 255, (n >> 8) & 255, 255 & n])),
                                    t.box(t.types.tfdt, new Uint8Array([1, 0, 0, 0, s >> 24, (s >> 16) & 255, (s >> 8) & 255, 255 & s, o >> 24, (o >> 16) & 255, (o >> 8) & 255, 255 & o])),
                                    t.trun(e, i.length + 16 + 20 + 8 + 16 + 8 + 8),
                                    i
                                );
                            }),
                            (t.trak = function (e) {
                                return (e.duration = e.duration || 4294967295), t.box(t.types.trak, t.tkhd(e), t.mdia(e));
                            }),
                            (t.trex = function (e) {
                                var r = e.id;
                                return t.box(t.types.trex, new Uint8Array([0, 0, 0, 0, r >> 24, (r >> 16) & 255, (r >> 8) & 255, 255 & r, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1]));
                            }),
                            (t.trun = function (e, r) {
                                var i,
                                    n,
                                    a,
                                    s,
                                    o,
                                    l,
                                    u = e.samples || [],
                                    h = u.length,
                                    d = 12 + 16 * h,
                                    c = new Uint8Array(d);
                                for (r += 8 + d, c.set([0, 0, 15, 1, (h >>> 24) & 255, (h >>> 16) & 255, (h >>> 8) & 255, 255 & h, (r >>> 24) & 255, (r >>> 16) & 255, (r >>> 8) & 255, 255 & r], 0), i = 0; i < h; i++)
                                    (a = (n = u[i]).duration),
                                        (s = n.size),
                                        (o = n.flags),
                                        (l = n.cts),
                                        c.set(
                                            [
                                                (a >>> 24) & 255,
                                                (a >>> 16) & 255,
                                                (a >>> 8) & 255,
                                                255 & a,
                                                (s >>> 24) & 255,
                                                (s >>> 16) & 255,
                                                (s >>> 8) & 255,
                                                255 & s,
                                                (o.isLeading << 2) | o.dependsOn,
                                                (o.isDependedOn << 6) | (o.hasRedundancy << 4) | (o.paddingValue << 1) | o.isNonSync,
                                                61440 & o.degradPrio,
                                                15 & o.degradPrio,
                                                (l >>> 24) & 255,
                                                (l >>> 16) & 255,
                                                (l >>> 8) & 255,
                                                255 & l,
                                            ],
                                            12 + 16 * i
                                        );
                                return t.box(t.types.trun, c);
                            }),
                            (t.initSegment = function (e) {
                                t.types || t.init();
                                var r = t.moov(e),
                                    i = new Uint8Array(t.FTYP.byteLength + r.byteLength);
                                return i.set(t.FTYP), i.set(r, t.FTYP.byteLength), i;
                            }),
                            t
                        );
                    })();
                (s.types = void 0), (s.HDLR_TYPES = void 0), (s.STTS = void 0), (s.STSC = void 0), (s.STCO = void 0), (s.STSZ = void 0), (s.VMHD = void 0), (s.SMHD = void 0), (s.STSD = void 0), (s.FTYP = void 0), (s.DINF = void 0);
                var o = s,
                    l = r(0),
                    u = r(2),
                    h = r(1),
                    d = r(4),
                    c = r(8);
                function f() {
                    return (f =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                var g = null,
                    v = null,
                    p = !1,
                    m = (function () {
                        function t(t, e, r, i) {
                            if (
                                (void 0 === i && (i = ""),
                                (this.observer = void 0),
                                (this.config = void 0),
                                (this.typeSupported = void 0),
                                (this.ISGenerated = !1),
                                (this._initPTS = void 0),
                                (this._initDTS = void 0),
                                (this.nextAvcDts = null),
                                (this.nextAudioPts = null),
                                (this.isAudioContiguous = !1),
                                (this.isVideoContiguous = !1),
                                (this.observer = t),
                                (this.config = e),
                                (this.typeSupported = r),
                                (this.ISGenerated = !1),
                                null === g)
                            ) {
                                var n = (navigator.userAgent || "").match(/Chrome\/(\d+)/i);
                                g = n ? parseInt(n[1]) : 0;
                            }
                            if (null === v) {
                                var a = navigator.userAgent.match(/Safari\/(\d+)/i);
                                v = a ? parseInt(a[1]) : 0;
                            }
                            p = (!!g && g < 75) || (!!v && v < 600);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {}),
                            (e.resetTimeStamp = function (t) {
                                h.b.log("[mp4-remuxer]: initPTS & initDTS reset"), (this._initPTS = this._initDTS = t);
                            }),
                            (e.resetNextTimestamp = function () {
                                h.b.log("[mp4-remuxer]: reset next timestamp"), (this.isVideoContiguous = !1), (this.isAudioContiguous = !1);
                            }),
                            (e.resetInitSegment = function () {
                                h.b.log("[mp4-remuxer]: ISGenerated flag reset"), (this.ISGenerated = !1);
                            }),
                            (e.getVideoStartPts = function (t) {
                                var e = !1,
                                    r = t.reduce(function (t, r) {
                                        var i = r.pts - t;
                                        return i < -4294967296 ? ((e = !0), y(t, r.pts)) : i > 0 ? t : r.pts;
                                    }, t[0].pts);
                                return e && h.b.debug("PTS rollover detected"), r;
                            }),
                            (e.remux = function (t, e, r, i, n, a, s, o) {
                                var l,
                                    u,
                                    c,
                                    f,
                                    g,
                                    v,
                                    p = n,
                                    m = n,
                                    T = t.pid > -1,
                                    b = e.pid > -1,
                                    E = e.samples.length,
                                    S = t.samples.length > 0,
                                    L = E > 1;
                                if (((!T || S) && (!b || L)) || this.ISGenerated || s) {
                                    this.ISGenerated || (c = this.generateIS(t, e, n));
                                    var A = this.isVideoContiguous,
                                        R = -1;
                                    if (
                                        L &&
                                        ((R = (function (t) {
                                            for (var e = 0; e < t.length; e++) if (t[e].key) return e;
                                            return -1;
                                        })(e.samples)),
                                        !A && this.config.forceKeyFrameOnDiscontinuity)
                                    )
                                        if (((v = !0), R > 0)) {
                                            h.b.warn("[mp4-remuxer]: Dropped " + R + " out of " + E + " video samples due to a missing keyframe");
                                            var D = this.getVideoStartPts(e.samples);
                                            (e.samples = e.samples.slice(R)), (e.dropped += R), (m += (e.samples[0].pts - D) / (e.timescale || 9e4));
                                        } else -1 === R && (h.b.warn("[mp4-remuxer]: No keyframe found out of " + E + " video samples"), (v = !1));
                                    if (this.ISGenerated) {
                                        if (S && L) {
                                            var k = this.getVideoStartPts(e.samples),
                                                _ = (y(t.samples[0].pts, k) - k) / e.inputTimeScale;
                                            (p += Math.max(0, _)), (m += Math.max(0, -_));
                                        }
                                        if (S) {
                                            if (
                                                (t.samplerate || (h.b.warn("[mp4-remuxer]: regenerate InitSegment as audio detected"), (c = this.generateIS(t, e, n))),
                                                (u = this.remuxAudio(t, p, this.isAudioContiguous, a, b || L || o === d.b.AUDIO ? m : void 0)),
                                                L)
                                            ) {
                                                var I = u ? u.endPTS - u.startPTS : 0;
                                                e.inputTimeScale || (h.b.warn("[mp4-remuxer]: regenerate InitSegment as video detected"), (c = this.generateIS(t, e, n))), (l = this.remuxVideo(e, m, A, I));
                                            }
                                        } else L && (l = this.remuxVideo(e, m, A, 0));
                                        l && ((l.firstKeyFrame = R), (l.independent = -1 !== R));
                                    }
                                }
                                return this.ISGenerated && (r.samples.length && (g = this.remuxID3(r, n)), i.samples.length && (f = this.remuxText(i, n))), { audio: u, video: l, initSegment: c, independent: v, text: f, id3: g };
                            }),
                            (e.generateIS = function (t, e, r) {
                                var n,
                                    a,
                                    s,
                                    l = t.samples,
                                    u = e.samples,
                                    h = this.typeSupported,
                                    d = {},
                                    c = !Object(i.a)(this._initPTS),
                                    f = "audio/mp4";
                                if (
                                    (c && (n = a = 1 / 0),
                                    t.config &&
                                        l.length &&
                                        ((t.timescale = t.samplerate),
                                        t.isAAC || (h.mpeg ? ((f = "audio/mpeg"), (t.codec = "")) : h.mp3 && (t.codec = "mp3")),
                                        (d.audio = { id: "audio", container: f, codec: t.codec, initSegment: !t.isAAC && h.mpeg ? new Uint8Array(0) : o.initSegment([t]), metadata: { channelCount: t.channelCount } }),
                                        c && ((s = t.inputTimeScale), (n = a = l[0].pts - Math.round(s * r)))),
                                    e.sps &&
                                        e.pps &&
                                        u.length &&
                                        ((e.timescale = e.inputTimeScale), (d.video = { id: "main", container: "video/mp4", codec: e.codec, initSegment: o.initSegment([e]), metadata: { width: e.width, height: e.height } }), c))
                                ) {
                                    s = e.inputTimeScale;
                                    var g = this.getVideoStartPts(u),
                                        v = Math.round(s * r);
                                    (a = Math.min(a, y(u[0].dts, g) - v)), (n = Math.min(n, g - v));
                                }
                                if (Object.keys(d).length) return (this.ISGenerated = !0), c && ((this._initPTS = n), (this._initDTS = a)), { tracks: d, initPTS: n, timescale: s };
                            }),
                            (e.remuxVideo = function (t, e, r, i) {
                                var n,
                                    a,
                                    s,
                                    d = t.inputTimeScale,
                                    v = t.samples,
                                    m = [],
                                    b = v.length,
                                    E = this._initPTS,
                                    S = this.nextAvcDts,
                                    L = 8,
                                    A = Number.POSITIVE_INFINITY,
                                    R = Number.NEGATIVE_INFINITY,
                                    D = 0,
                                    k = !1;
                                (r && null !== S) || (S = e * d - (v[0].pts - y(v[0].dts, v[0].pts)));
                                for (var _ = 0; _ < b; _++) {
                                    var I = v[_];
                                    if (((I.pts = y(I.pts - E, S)), (I.dts = y(I.dts - E, S)), I.dts > I.pts)) {
                                        D = Math.max(Math.min(D, I.pts - I.dts), -18e3);
                                    }
                                    I.dts < v[_ > 0 ? _ - 1 : _].dts && (k = !0);
                                }
                                k &&
                                    v.sort(function (t, e) {
                                        var r = t.dts - e.dts,
                                            i = t.pts - e.pts;
                                        return r || i;
                                    }),
                                    (a = v[0].dts),
                                    (s = v[v.length - 1].dts);
                                var C = Math.round((s - a) / (b - 1));
                                if (D < 0) {
                                    if (D < -2 * C) {
                                        h.b.warn("PTS < DTS detected in video samples, offsetting DTS from PTS by " + Object(c.b)(-C, !0) + " ms");
                                        for (var w = D, O = 0; O < b; O++) (v[O].dts = w = Math.max(w, v[O].pts - C)), (v[O].pts = Math.max(w, v[O].pts));
                                    } else {
                                        h.b.warn("PTS < DTS detected in video samples, shifting DTS by " + Object(c.b)(D, !0) + " ms to overcome this issue");
                                        for (var x = 0; x < b; x++) v[x].dts = v[x].dts + D;
                                    }
                                    a = v[0].dts;
                                }
                                if (r) {
                                    var P = a - S,
                                        M = P > C;
                                    if (M || P < -1) {
                                        M
                                            ? h.b.warn("AVC: " + Object(c.b)(P, !0) + " ms (" + P + "dts) hole between fragments detected, filling it")
                                            : h.b.warn("AVC: " + Object(c.b)(-P, !0) + " ms (" + P + "dts) overlapping between fragments detected"),
                                            (a = S);
                                        var F = v[0].pts - P;
                                        (v[0].dts = a), (v[0].pts = F), h.b.log("Video: First PTS/DTS adjusted: " + Object(c.b)(F, !0) + "/" + Object(c.b)(a, !0) + ", delta: " + Object(c.b)(P, !0) + " ms");
                                    }
                                }
                                p && (a = Math.max(0, a));
                                for (var N = 0, U = 0, B = 0; B < b; B++) {
                                    for (var G = v[B], K = G.units, j = K.length, H = 0, V = 0; V < j; V++) H += K[V].data.length;
                                    (U += H), (N += j), (G.length = H), (G.dts = Math.max(G.dts, a)), (G.pts = Math.max(G.pts, G.dts, 0)), (A = Math.min(G.pts, A)), (R = Math.max(G.pts, R));
                                }
                                s = v[b - 1].dts;
                                var W,
                                    Y = U + 4 * N + 8;
                                try {
                                    W = new Uint8Array(Y);
                                } catch (t) {
                                    return void this.observer.emit(l.a.ERROR, l.a.ERROR, { type: u.b.MUX_ERROR, details: u.a.REMUX_ALLOC_ERROR, fatal: !1, bytes: Y, reason: "fail allocating video mdat " + Y });
                                }
                                var q = new DataView(W.buffer);
                                q.setUint32(0, Y), W.set(o.types.mdat, 4);
                                for (var X = 0; X < b; X++) {
                                    for (var z = v[X], Q = z.units, $ = 0, J = 0, Z = Q.length; J < Z; J++) {
                                        var tt = Q[J],
                                            et = tt.data,
                                            rt = tt.data.byteLength;
                                        q.setUint32(L, rt), (L += 4), W.set(et, L), (L += rt), ($ += 4 + rt);
                                    }
                                    if (X < b - 1) n = v[X + 1].dts - z.dts;
                                    else {
                                        var it = this.config,
                                            nt = z.dts - v[X > 0 ? X - 1 : X].dts;
                                        if (it.stretchShortVideoTrack && null !== this.nextAudioPts) {
                                            var at = Math.floor(it.maxBufferHole * d),
                                                st = (i ? A + i * d : this.nextAudioPts) - z.pts;
                                            st > at
                                                ? ((n = st - nt) < 0 && (n = nt), h.b.log("[mp4-remuxer]: It is approximately " + st / 90 + " ms to the next segment; using duration " + n / 90 + " ms for the last video frame."))
                                                : (n = nt);
                                        } else n = nt;
                                    }
                                    var ot = Math.round(z.pts - z.dts);
                                    m.push(new T(z.key, n, $, ot));
                                }
                                if (m.length && g && g < 70) {
                                    var lt = m[0].flags;
                                    (lt.dependsOn = 2), (lt.isNonSync = 0);
                                }
                                (this.nextAvcDts = S = s + n), (this.isVideoContiguous = !0);
                                var ut = {
                                    data1: o.moof(t.sequenceNumber++, a, f({}, t, { samples: m })),
                                    data2: W,
                                    startPTS: A / d,
                                    endPTS: (R + n) / d,
                                    startDTS: a / d,
                                    endDTS: S / d,
                                    type: "video",
                                    hasAudio: !1,
                                    hasVideo: !0,
                                    nb: m.length,
                                    dropped: t.dropped,
                                };
                                return (t.samples = []), (t.dropped = 0), ut;
                            }),
                            (e.remuxAudio = function (t, e, r, i, a) {
                                var s = t.inputTimeScale,
                                    d = s / (t.samplerate ? t.samplerate : s),
                                    c = t.isAAC ? 1024 : 1152,
                                    g = c * d,
                                    v = this._initPTS,
                                    p = !t.isAAC && this.typeSupported.mpeg,
                                    m = [],
                                    b = t.samples,
                                    E = p ? 0 : 8,
                                    S = this.nextAudioPts || -1,
                                    L = e * s;
                                if (
                                    ((this.isAudioContiguous = r = r || (b.length && S > 0 && ((i && Math.abs(L - S) < 9e3) || Math.abs(y(b[0].pts - v, L) - S) < 20 * g))),
                                    b.forEach(function (t) {
                                        t.pts = y(t.pts - v, L);
                                    }),
                                    !r || S < 0)
                                ) {
                                    if (
                                        !(b = b.filter(function (t) {
                                            return t.pts >= 0;
                                        })).length
                                    )
                                        return;
                                    S = 0 === a ? 0 : i ? Math.max(0, L) : b[0].pts;
                                }
                                if (t.isAAC)
                                    for (var A = void 0 !== a, R = this.config.maxAudioFramesDrift, D = 0, k = S; D < b.length; D++) {
                                        var _ = b[D],
                                            I = _.pts,
                                            C = I - k,
                                            w = Math.abs((1e3 * C) / s);
                                        if (C <= -R * g && A) 0 === D && (h.b.warn("Audio frame @ " + (I / s).toFixed(3) + "s overlaps nextAudioPts by " + Math.round((1e3 * C) / s) + " ms."), (this.nextAudioPts = S = k = I));
                                        else if (C >= R * g && w < 1e4 && A) {
                                            var O = Math.round(C / g);
                                            (k = I - O * g) < 0 && (O--, (k += g)),
                                                0 === D && (this.nextAudioPts = S = k),
                                                h.b.warn("[mp4-remuxer]: Injecting " + O + " audio frame @ " + (k / s).toFixed(3) + "s due to " + Math.round((1e3 * C) / s) + " ms gap.");
                                            for (var x = 0; x < O; x++) {
                                                var P = Math.max(k, 0),
                                                    M = n.getSilentFrame(t.manifestCodec || t.codec, t.channelCount);
                                                M || (h.b.log("[mp4-remuxer]: Unable to get silent frame for given audio codec; duplicating last frame instead."), (M = _.unit.subarray())), b.splice(D, 0, { unit: M, pts: P }), (k += g), D++;
                                            }
                                        }
                                        (_.pts = k), (k += g);
                                    }
                                for (var F, N = null, U = null, B = 0, G = b.length; G--; ) B += b[G].unit.byteLength;
                                for (var K = 0, j = b.length; K < j; K++) {
                                    var H = b[K],
                                        V = H.unit,
                                        W = H.pts;
                                    if (null !== U) {
                                        m[K - 1].duration = Math.round((W - U) / d);
                                    } else {
                                        if ((r && t.isAAC && (W = S), (N = W), !(B > 0))) return;
                                        B += E;
                                        try {
                                            F = new Uint8Array(B);
                                        } catch (t) {
                                            return void this.observer.emit(l.a.ERROR, l.a.ERROR, { type: u.b.MUX_ERROR, details: u.a.REMUX_ALLOC_ERROR, fatal: !1, bytes: B, reason: "fail allocating audio mdat " + B });
                                        }
                                        p || (new DataView(F.buffer).setUint32(0, B), F.set(o.types.mdat, 4));
                                    }
                                    F.set(V, E);
                                    var Y = V.byteLength;
                                    (E += Y), m.push(new T(!0, c, Y, 0)), (U = W);
                                }
                                var q = m.length;
                                if (q) {
                                    var X = m[m.length - 1];
                                    this.nextAudioPts = S = U + d * X.duration;
                                    var z = p ? new Uint8Array(0) : o.moof(t.sequenceNumber++, N / d, f({}, t, { samples: m }));
                                    t.samples = [];
                                    var Q = N / s,
                                        $ = S / s,
                                        J = { data1: z, data2: F, startPTS: Q, endPTS: $, startDTS: Q, endDTS: $, type: "audio", hasAudio: !0, hasVideo: !1, nb: q };
                                    return (this.isAudioContiguous = !0), J;
                                }
                            }),
                            (e.remuxEmptyAudio = function (t, e, r, i) {
                                var a = t.inputTimeScale,
                                    s = a / (t.samplerate ? t.samplerate : a),
                                    o = this.nextAudioPts,
                                    l = (null !== o ? o : i.startDTS * a) + this._initDTS,
                                    u = i.endDTS * a + this._initDTS,
                                    d = 1024 * s,
                                    c = Math.ceil((u - l) / d),
                                    f = n.getSilentFrame(t.manifestCodec || t.codec, t.channelCount);
                                if ((h.b.warn("[mp4-remuxer]: remux empty Audio"), f)) {
                                    for (var g = [], v = 0; v < c; v++) {
                                        var p = l + v * d;
                                        g.push({ unit: f, pts: p, dts: p });
                                    }
                                    return (t.samples = g), this.remuxAudio(t, e, r, !1);
                                }
                                h.b.trace("[mp4-remuxer]: Unable to remuxEmptyAudio since we were unable to get a silent frame for given audio codec");
                            }),
                            (e.remuxID3 = function (t, e) {
                                var r = t.samples.length;
                                if (r) {
                                    for (var i = t.inputTimeScale, n = this._initPTS, a = this._initDTS, s = 0; s < r; s++) {
                                        var o = t.samples[s];
                                        (o.pts = y(o.pts - n, e * i) / i), (o.dts = y(o.dts - a, e * i) / i);
                                    }
                                    var l = t.samples;
                                    return (t.samples = []), { samples: l };
                                }
                            }),
                            (e.remuxText = function (t, e) {
                                var r = t.samples.length;
                                if (r) {
                                    for (var i = t.inputTimeScale, n = this._initPTS, a = 0; a < r; a++) {
                                        var s = t.samples[a];
                                        s.pts = y(s.pts - n, e * i) / i;
                                    }
                                    t.samples.sort(function (t, e) {
                                        return t.pts - e.pts;
                                    });
                                    var o = t.samples;
                                    return (t.samples = []), { samples: o };
                                }
                            }),
                            t
                        );
                    })();
                function y(t, e) {
                    var r;
                    if (null === e) return t;
                    for (r = e < t ? -8589934592 : 8589934592; Math.abs(t - e) > 4294967296; ) t += r;
                    return t;
                }
                var T = function (t, e, r, i) {
                        (this.size = void 0), (this.duration = void 0), (this.cts = void 0), (this.flags = void 0), (this.duration = e), (this.size = r), (this.cts = i), (this.flags = new b(t));
                    },
                    b = function (t) {
                        (this.isLeading = 0), (this.isDependedOn = 0), (this.hasRedundancy = 0), (this.degradPrio = 0), (this.dependsOn = 1), (this.isNonSync = 1), (this.dependsOn = t ? 2 : 1), (this.isNonSync = t ? 0 : 1);
                    };
            },
            function (t, e, r) {
                "use strict";
                r.d(e, "a", function () {
                    return a;
                });
                var i = r(11);
                function n(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var a = (function () {
                    function t(t, e) {
                        (this._uri = null),
                            (this.method = null),
                            (this.keyFormat = null),
                            (this.keyFormatVersions = null),
                            (this.keyID = null),
                            (this.key = null),
                            (this.iv = null),
                            (this._uri = e ? Object(i.buildAbsoluteURL)(t, e, { alwaysNormalize: !0 }) : t);
                    }
                    var e, r, a;
                    return (
                        (t.fromURL = function (e, r) {
                            return new t(e, r);
                        }),
                        (t.fromURI = function (e) {
                            return new t(e);
                        }),
                        (e = t),
                        (r = [
                            {
                                key: "uri",
                                get: function () {
                                    return this._uri;
                                },
                            },
                        ]) && n(e.prototype, r),
                        a && n(e, a),
                        t
                    );
                })();
            },
            function (t, e, r) {
                function i(t) {
                    var e = {};
                    function r(i) {
                        if (e[i]) return e[i].exports;
                        var n = (e[i] = { i: i, l: !1, exports: {} });
                        return t[i].call(n.exports, n, n.exports, r), (n.l = !0), n.exports;
                    }
                    (r.m = t),
                        (r.c = e),
                        (r.i = function (t) {
                            return t;
                        }),
                        (r.d = function (t, e, i) {
                            r.o(t, e) || Object.defineProperty(t, e, { configurable: !1, enumerable: !0, get: i });
                        }),
                        (r.r = function (t) {
                            Object.defineProperty(t, "__esModule", { value: !0 });
                        }),
                        (r.n = function (t) {
                            var e =
                                t && t.__esModule
                                    ? function () {
                                          return t.default;
                                      }
                                    : function () {
                                          return t;
                                      };
                            return r.d(e, "a", e), e;
                        }),
                        (r.o = function (t, e) {
                            return Object.prototype.hasOwnProperty.call(t, e);
                        }),
                        (r.p = "/"),
                        (r.oe = function (t) {
                            throw (console.error(t), t);
                        });
                    var i = r((r.s = ENTRY_MODULE));
                    return i.default || i;
                }
                function n(t) {
                    return (t + "").replace(/[.?*+^$[\]\\(){}|-]/g, "\\$&");
                }
                function a(t, e, i) {
                    var a = {};
                    a[i] = [];
                    var s = e.toString(),
                        o = s.match(/^function\s?\w*\(\w+,\s*\w+,\s*(\w+)\)/);
                    if (!o) return a;
                    for (var l, u = o[1], h = new RegExp("(\\\\n|\\W)" + n(u) + "\\(\\s*(/\\*.*?\\*/)?\\s*.*?([\\.|\\-|\\+|\\w|/|@]+).*?\\)", "g"); (l = h.exec(s)); ) "dll-reference" !== l[3] && a[i].push(l[3]);
                    for (h = new RegExp("\\(" + n(u) + '\\("(dll-reference\\s([\\.|\\-|\\+|\\w|/|@]+))"\\)\\)\\(\\s*(/\\*.*?\\*/)?\\s*.*?([\\.|\\-|\\+|\\w|/|@]+).*?\\)', "g"); (l = h.exec(s)); )
                        t[l[2]] || (a[i].push(l[1]), (t[l[2]] = r(l[1]).m)), (a[l[2]] = a[l[2]] || []), a[l[2]].push(l[4]);
                    for (var d, c = Object.keys(a), f = 0; f < c.length; f++) for (var g = 0; g < a[c[f]].length; g++) (d = a[c[f]][g]), isNaN(1 * d) || (a[c[f]][g] = 1 * a[c[f]][g]);
                    return a;
                }
                function s(t) {
                    return Object.keys(t).reduce(function (e, r) {
                        return e || t[r].length > 0;
                    }, !1);
                }
                t.exports = function (t, e) {
                    e = e || {};
                    var n = { main: r.m },
                        o = e.all
                            ? { main: Object.keys(n.main) }
                            : (function (t, e) {
                                  for (var r = { main: [e] }, i = { main: [] }, n = { main: {} }; s(r); )
                                      for (var o = Object.keys(r), l = 0; l < o.length; l++) {
                                          var u = o[l],
                                              h = r[u].pop();
                                          if (((n[u] = n[u] || {}), !n[u][h] && t[u][h])) {
                                              (n[u][h] = !0), (i[u] = i[u] || []), i[u].push(h);
                                              for (var d = a(t, t[u][h], u), c = Object.keys(d), f = 0; f < c.length; f++) (r[c[f]] = r[c[f]] || []), (r[c[f]] = r[c[f]].concat(d[c[f]]));
                                          }
                                      }
                                  return i;
                              })(n, t),
                        l = "";
                    Object.keys(o)
                        .filter(function (t) {
                            return "main" !== t;
                        })
                        .forEach(function (t) {
                            for (var e = 0; o[t][e]; ) e++;
                            o[t].push(e),
                                (n[t][e] = "(function(module, exports, __webpack_require__) { module.exports = __webpack_require__; })"),
                                (l =
                                    l +
                                    "var " +
                                    t +
                                    " = (" +
                                    i.toString().replace("ENTRY_MODULE", JSON.stringify(e)) +
                                    ")({" +
                                    o[t]
                                        .map(function (e) {
                                            return JSON.stringify(e) + ": " + n[t][e].toString();
                                        })
                                        .join(",") +
                                    "});\n");
                        }),
                        (l =
                            l +
                            "new ((" +
                            i.toString().replace("ENTRY_MODULE", JSON.stringify(t)) +
                            ")({" +
                            o.main
                                .map(function (t) {
                                    return JSON.stringify(t) + ": " + n.main[t].toString();
                                })
                                .join(",") +
                            "}))(self);");
                    var u = new window.Blob([l], { type: "text/javascript" });
                    if (e.bare) return u;
                    var h = (window.URL || window.webkitURL || window.mozURL || window.msURL).createObjectURL(u),
                        d = new window.Worker(h);
                    return (d.objectURL = h), d;
                };
            },
            function (t, e, r) {
                "use strict";
                r.r(e),
                    r.d(e, "default", function () {
                        return o;
                    });
                var i = r(10),
                    n = r(0),
                    a = r(1),
                    s = r(14);
                function o(t) {
                    var e = new s.EventEmitter(),
                        r = function (e, r) {
                            t.postMessage({ event: e, data: r });
                        };
                    e.on(n.a.FRAG_DECRYPTED, r),
                        e.on(n.a.ERROR, r),
                        t.addEventListener("message", function (n) {
                            var s = n.data;
                            switch (s.cmd) {
                                case "init":
                                    var o = JSON.parse(s.config);
                                    (t.transmuxer = new i.c(e, s.typeSupported, o, s.vendor, s.id)), Object(a.a)(o.debug), r("init", null);
                                    break;
                                case "configure":
                                    t.transmuxer.configure(s.config);
                                    break;
                                case "demux":
                                    var u = t.transmuxer.push(s.data, s.decryptdata, s.chunkMeta, s.state);
                                    Object(i.d)(u)
                                        ? u.then(function (e) {
                                              l(t, e);
                                          })
                                        : l(t, u);
                                    break;
                                case "flush":
                                    var d = s.chunkMeta,
                                        c = t.transmuxer.flush(d);
                                    Object(i.d)(c)
                                        ? c.then(function (e) {
                                              h(t, e, d);
                                          })
                                        : h(t, c, d);
                            }
                        });
                }
                function l(t, e) {
                    if ((r = e.remuxResult).audio || r.video || r.text || r.id3 || r.initSegment) {
                        var r,
                            i = [],
                            n = e.remuxResult,
                            a = n.audio,
                            s = n.video;
                        a && u(i, a), s && u(i, s), t.postMessage({ event: "transmuxComplete", data: e }, i);
                    }
                }
                function u(t, e) {
                    e.data1 && t.push(e.data1.buffer), e.data2 && t.push(e.data2.buffer);
                }
                function h(t, e, r) {
                    e.forEach(function (e) {
                        l(t, e);
                    }),
                        t.postMessage({ event: "flush", data: r });
                }
            },
            function (t, e, r) {
                "use strict";
                r.r(e),
                    r.d(e, "default", function () {
                        return ii;
                    });
                var i = r(11),
                    n = r(3),
                    a = r(0),
                    s = r(2),
                    o = r(1),
                    l = r(6),
                    u = r(5);
                function h(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var d = (function () {
                        function t(t) {
                            (this.PTSKnown = !1),
                                (this.alignedSliding = !1),
                                (this.averagetargetduration = void 0),
                                (this.endCC = 0),
                                (this.endSN = 0),
                                (this.fragments = void 0),
                                (this.fragmentHint = void 0),
                                (this.partList = null),
                                (this.live = !0),
                                (this.ageHeader = 0),
                                (this.advancedDateTime = void 0),
                                (this.updated = !0),
                                (this.advanced = !0),
                                (this.availabilityDelay = void 0),
                                (this.misses = 0),
                                (this.needSidxRanges = !1),
                                (this.startCC = 0),
                                (this.startSN = 0),
                                (this.startTimeOffset = null),
                                (this.targetduration = 0),
                                (this.totalduration = 0),
                                (this.type = null),
                                (this.url = void 0),
                                (this.m3u8 = ""),
                                (this.version = null),
                                (this.canBlockReload = !1),
                                (this.canSkipUntil = 0),
                                (this.canSkipDateRanges = !1),
                                (this.skippedSegments = 0),
                                (this.recentlyRemovedDateranges = void 0),
                                (this.partHoldBack = 0),
                                (this.holdBack = 0),
                                (this.partTarget = 0),
                                (this.preloadHint = void 0),
                                (this.renditionReports = void 0),
                                (this.tuneInGoal = 0),
                                (this.deltaUpdateFailed = void 0),
                                (this.driftStartTime = 0),
                                (this.driftEndTime = 0),
                                (this.driftStart = 0),
                                (this.driftEnd = 0),
                                (this.fragments = []),
                                (this.url = t);
                        }
                        var e, r, i;
                        return (
                            (t.prototype.reloaded = function (t) {
                                if (!t) return (this.advanced = !0), void (this.updated = !0);
                                var e = this.lastPartSn - t.lastPartSn,
                                    r = this.lastPartIndex - t.lastPartIndex;
                                (this.updated = this.endSN !== t.endSN || !!r || !!e),
                                    (this.advanced = this.endSN > t.endSN || e > 0 || (0 === e && r > 0)),
                                    this.updated || this.advanced ? (this.misses = Math.floor(0.6 * t.misses)) : (this.misses = t.misses + 1),
                                    (this.availabilityDelay = t.availabilityDelay);
                            }),
                            (e = t),
                            (r = [
                                {
                                    key: "hasProgramDateTime",
                                    get: function () {
                                        return !!this.fragments.length && Object(n.a)(this.fragments[this.fragments.length - 1].programDateTime);
                                    },
                                },
                                {
                                    key: "levelTargetDuration",
                                    get: function () {
                                        return this.averagetargetduration || this.targetduration || 10;
                                    },
                                },
                                {
                                    key: "drift",
                                    get: function () {
                                        var t = this.driftEndTime - this.driftStartTime;
                                        return t > 0 ? (1e3 * (this.driftEnd - this.driftStart)) / t : 1;
                                    },
                                },
                                {
                                    key: "edge",
                                    get: function () {
                                        return this.partEnd || this.fragmentEnd;
                                    },
                                },
                                {
                                    key: "partEnd",
                                    get: function () {
                                        var t;
                                        return null !== (t = this.partList) && void 0 !== t && t.length ? this.partList[this.partList.length - 1].end : this.fragmentEnd;
                                    },
                                },
                                {
                                    key: "fragmentEnd",
                                    get: function () {
                                        var t;
                                        return null !== (t = this.fragments) && void 0 !== t && t.length ? this.fragments[this.fragments.length - 1].end : 0;
                                    },
                                },
                                {
                                    key: "age",
                                    get: function () {
                                        return this.advancedDateTime ? Math.max(Date.now() - this.advancedDateTime, 0) / 1e3 : 0;
                                    },
                                },
                                {
                                    key: "lastPartIndex",
                                    get: function () {
                                        var t;
                                        return null !== (t = this.partList) && void 0 !== t && t.length ? this.partList[this.partList.length - 1].index : -1;
                                    },
                                },
                                {
                                    key: "lastPartSn",
                                    get: function () {
                                        var t;
                                        return null !== (t = this.partList) && void 0 !== t && t.length ? this.partList[this.partList.length - 1].fragment.sn : this.endSN;
                                    },
                                },
                            ]) && h(e.prototype, r),
                            i && h(e, i),
                            t
                        );
                    })(),
                    c = r(17),
                    f = /^(\d+)x(\d+)$/,
                    g = /\s*(.+?)\s*=((?:\".*?\")|.*?)(?:,|$)/g,
                    v = (function () {
                        function t(e) {
                            for (var r in ("string" == typeof e && (e = t.parseAttrList(e)), e)) e.hasOwnProperty(r) && (this[r] = e[r]);
                        }
                        var e = t.prototype;
                        return (
                            (e.decimalInteger = function (t) {
                                var e = parseInt(this[t], 10);
                                return e > Number.MAX_SAFE_INTEGER ? 1 / 0 : e;
                            }),
                            (e.hexadecimalInteger = function (t) {
                                if (this[t]) {
                                    var e = (this[t] || "0x").slice(2);
                                    e = (1 & e.length ? "0" : "") + e;
                                    for (var r = new Uint8Array(e.length / 2), i = 0; i < e.length / 2; i++) r[i] = parseInt(e.slice(2 * i, 2 * i + 2), 16);
                                    return r;
                                }
                                return null;
                            }),
                            (e.hexadecimalIntegerAsNumber = function (t) {
                                var e = parseInt(this[t], 16);
                                return e > Number.MAX_SAFE_INTEGER ? 1 / 0 : e;
                            }),
                            (e.decimalFloatingPoint = function (t) {
                                return parseFloat(this[t]);
                            }),
                            (e.optionalFloat = function (t, e) {
                                var r = this[t];
                                return r ? parseFloat(r) : e;
                            }),
                            (e.enumeratedString = function (t) {
                                return this[t];
                            }),
                            (e.bool = function (t) {
                                return "YES" === this[t];
                            }),
                            (e.decimalResolution = function (t) {
                                var e = f.exec(this[t]);
                                if (null !== e) return { width: parseInt(e[1], 10), height: parseInt(e[2], 10) };
                            }),
                            (t.parseAttrList = function (t) {
                                var e,
                                    r = {};
                                for (g.lastIndex = 0; null !== (e = g.exec(t)); ) {
                                    var i = e[2];
                                    0 === i.indexOf('"') && i.lastIndexOf('"') === i.length - 1 && (i = i.slice(1, -1)), (r[e[1]] = i);
                                }
                                return r;
                            }),
                            t
                        );
                    })(),
                    p = {
                        audio: {
                            a3ds: !0,
                            "ac-3": !0,
                            "ac-4": !0,
                            alac: !0,
                            alaw: !0,
                            dra1: !0,
                            "dts+": !0,
                            "dts-": !0,
                            dtsc: !0,
                            dtse: !0,
                            dtsh: !0,
                            "ec-3": !0,
                            enca: !0,
                            g719: !0,
                            g726: !0,
                            m4ae: !0,
                            mha1: !0,
                            mha2: !0,
                            mhm1: !0,
                            mhm2: !0,
                            mlpa: !0,
                            mp4a: !0,
                            "raw ": !0,
                            Opus: !0,
                            samr: !0,
                            sawb: !0,
                            sawp: !0,
                            sevc: !0,
                            sqcp: !0,
                            ssmv: !0,
                            twos: !0,
                            ulaw: !0,
                        },
                        video: {
                            avc1: !0,
                            avc2: !0,
                            avc3: !0,
                            avc4: !0,
                            avcp: !0,
                            av01: !0,
                            drac: !0,
                            dvav: !0,
                            dvhe: !0,
                            encv: !0,
                            hev1: !0,
                            hvc1: !0,
                            mjp2: !0,
                            mp4v: !0,
                            mvc1: !0,
                            mvc2: !0,
                            mvc3: !0,
                            mvc4: !0,
                            resv: !0,
                            rv60: !0,
                            s263: !0,
                            svc1: !0,
                            svc2: !0,
                            "vc-1": !0,
                            vp08: !0,
                            vp09: !0,
                        },
                        text: { stpp: !0, wvtt: !0 },
                    };
                function m(t, e) {
                    return MediaSource.isTypeSupported((e || "video") + '/mp4;codecs="' + t + '"');
                }
                var y = /#EXT-X-STREAM-INF:([^\r\n]*)(?:[\r\n](?:#[^\r\n]*)?)*([^\r\n]+)|#EXT-X-SESSION-DATA:([^\r\n]*)[\r\n]+/g,
                    T = /#EXT-X-MEDIA:(.*)/g,
                    b = new RegExp([/#EXTINF:\s*(\d*(?:\.\d+)?)(?:,(.*)\s+)?/.source, /(?!#) *(\S[\S ]*)/.source, /#EXT-X-BYTERANGE:*(.+)/.source, /#EXT-X-PROGRAM-DATE-TIME:(.+)/.source, /#.*/.source].join("|"), "g"),
                    E = new RegExp(
                        [
                            /#(EXTM3U)/.source,
                            /#EXT-X-(PLAYLIST-TYPE):(.+)/.source,
                            /#EXT-X-(MEDIA-SEQUENCE): *(\d+)/.source,
                            /#EXT-X-(SKIP):(.+)/.source,
                            /#EXT-X-(TARGETDURATION): *(\d+)/.source,
                            /#EXT-X-(KEY):(.+)/.source,
                            /#EXT-X-(START):(.+)/.source,
                            /#EXT-X-(ENDLIST)/.source,
                            /#EXT-X-(DISCONTINUITY-SEQ)UENCE: *(\d+)/.source,
                            /#EXT-X-(DIS)CONTINUITY/.source,
                            /#EXT-X-(VERSION):(\d+)/.source,
                            /#EXT-X-(MAP):(.+)/.source,
                            /#EXT-X-(SERVER-CONTROL):(.+)/.source,
                            /#EXT-X-(PART-INF):(.+)/.source,
                            /#EXT-X-(GAP)/.source,
                            /#EXT-X-(BITRATE):\s*(\d+)/.source,
                            /#EXT-X-(PART):(.+)/.source,
                            /#EXT-X-(PRELOAD-HINT):(.+)/.source,
                            /#EXT-X-(RENDITION-REPORT):(.+)/.source,
                            /(#)([^:]*):(.*)/.source,
                            /(#)(.*)(?:.*)\r?\n?/.source,
                        ].join("|")
                    ),
                    S = /\.(mp4|m4s|m4v|m4a)$/i;
                var L = (function () {
                    function t() {}
                    return (
                        (t.findGroup = function (t, e) {
                            for (var r = 0; r < t.length; r++) {
                                var i = t[r];
                                if (i.id === e) return i;
                            }
                        }),
                        (t.convertAVC1ToAVCOTI = function (t) {
                            var e = t.split(".");
                            if (e.length > 2) {
                                var r = e.shift() + ".";
                                return (r += parseInt(e.shift()).toString(16)), (r += ("000" + parseInt(e.shift()).toString(16)).substr(-4));
                            }
                            return t;
                        }),
                        (t.resolve = function (t, e) {
                            return i.buildAbsoluteURL(e, t, { alwaysNormalize: !0 });
                        }),
                        (t.parseMasterPlaylist = function (e, r) {
                            var i,
                                n = [],
                                a = {},
                                s = !1;
                            for (y.lastIndex = 0; null != (i = y.exec(e)); )
                                if (i[1]) {
                                    var o = new v(i[1]),
                                        l = { attrs: o, bitrate: o.decimalInteger("AVERAGE-BANDWIDTH") || o.decimalInteger("BANDWIDTH"), name: o.NAME, url: t.resolve(i[2], r) },
                                        u = o.decimalResolution("RESOLUTION");
                                    u && ((l.width = u.width), (l.height = u.height)),
                                        A(
                                            (o.CODECS || "").split(/[ ,]+/).filter(function (t) {
                                                return t;
                                            }),
                                            l
                                        ),
                                        l.videoCodec && -1 !== l.videoCodec.indexOf("avc1") && (l.videoCodec = t.convertAVC1ToAVCOTI(l.videoCodec)),
                                        n.push(l);
                                } else if (i[3]) {
                                    var h = new v(i[3]);
                                    h["DATA-ID"] && ((s = !0), (a[h["DATA-ID"]] = h));
                                }
                            return { levels: n, sessionData: s ? a : null };
                        }),
                        (t.parseMasterPlaylistMedia = function (e, r, i, n) {
                            var a;
                            void 0 === n && (n = []);
                            var s = [],
                                o = 0;
                            for (T.lastIndex = 0; null !== (a = T.exec(e)); ) {
                                var l = new v(a[1]);
                                if (l.TYPE === i) {
                                    var u = {
                                        attrs: l,
                                        bitrate: 0,
                                        id: o++,
                                        groupId: l["GROUP-ID"],
                                        instreamId: l["INSTREAM-ID"],
                                        name: l.NAME || l.LANGUAGE || "",
                                        type: i,
                                        default: l.bool("DEFAULT"),
                                        autoselect: l.bool("AUTOSELECT"),
                                        forced: l.bool("FORCED"),
                                        lang: l.LANGUAGE,
                                        url: l.URI ? t.resolve(l.URI, r) : "",
                                    };
                                    if (n.length) {
                                        var h = t.findGroup(n, u.groupId) || n[0];
                                        R(u, h, "audioCodec"), R(u, h, "textCodec");
                                    }
                                    s.push(u);
                                }
                            }
                            return s;
                        }),
                        (t.parseLevelPlaylist = function (t, e, r, a, s) {
                            var l,
                                h,
                                f,
                                g = new d(e),
                                p = g.fragments,
                                m = null,
                                y = 0,
                                T = 0,
                                L = 0,
                                A = 0,
                                R = null,
                                k = new u.b(a, e),
                                _ = -1,
                                I = !1;
                            for (b.lastIndex = 0, g.m3u8 = t; null !== (l = b.exec(t)); ) {
                                I && ((I = !1), ((k = new u.b(a, e)).start = L), (k.sn = y), (k.cc = A), (k.level = r), m && ((k.initSegment = m), (k.rawProgramDateTime = m.rawProgramDateTime)));
                                var C = l[1];
                                if (C) {
                                    k.duration = parseFloat(C);
                                    var w = (" " + l[2]).slice(1);
                                    (k.title = w || null), k.tagList.push(w ? ["INF", C, w] : ["INF", C]);
                                } else if (l[3])
                                    Object(n.a)(k.duration) &&
                                        ((k.start = L),
                                        f && (k.levelkey = f),
                                        (k.sn = y),
                                        (k.level = r),
                                        (k.cc = A),
                                        (k.urlId = s),
                                        p.push(k),
                                        (k.relurl = (" " + l[3]).slice(1)),
                                        D(k, R),
                                        (R = k),
                                        (L += k.duration),
                                        y++,
                                        (T = 0),
                                        (I = !0));
                                else if (l[4]) {
                                    var O = (" " + l[4]).slice(1);
                                    R ? k.setByteRange(O, R) : k.setByteRange(O);
                                } else if (l[5]) (k.rawProgramDateTime = (" " + l[5]).slice(1)), k.tagList.push(["PROGRAM-DATE-TIME", k.rawProgramDateTime]), -1 === _ && (_ = p.length);
                                else {
                                    if (!(l = l[0].match(E))) {
                                        o.b.warn("No matches on slow regex match for level playlist!");
                                        continue;
                                    }
                                    for (h = 1; h < l.length && void 0 === l[h]; h++);
                                    var x = (" " + l[h]).slice(1),
                                        P = (" " + l[h + 1]).slice(1),
                                        M = l[h + 2] ? (" " + l[h + 2]).slice(1) : "";
                                    switch (x) {
                                        case "PLAYLIST-TYPE":
                                            g.type = P.toUpperCase();
                                            break;
                                        case "MEDIA-SEQUENCE":
                                            y = g.startSN = parseInt(P);
                                            break;
                                        case "SKIP":
                                            var F = new v(P),
                                                N = F.decimalInteger("SKIPPED-SEGMENTS");
                                            if (Object(n.a)(N)) {
                                                g.skippedSegments = N;
                                                for (var U = N; U--; ) p.unshift(null);
                                                y += N;
                                            }
                                            var B = F.enumeratedString("RECENTLY-REMOVED-DATERANGES");
                                            B && (g.recentlyRemovedDateranges = B.split("\t"));
                                            break;
                                        case "TARGETDURATION":
                                            g.targetduration = parseFloat(P);
                                            break;
                                        case "VERSION":
                                            g.version = parseInt(P);
                                            break;
                                        case "EXTM3U":
                                            break;
                                        case "ENDLIST":
                                            g.live = !1;
                                            break;
                                        case "#":
                                            (P || M) && k.tagList.push(M ? [P, M] : [P]);
                                            break;
                                        case "DIS":
                                            A++;
                                        case "GAP":
                                            k.tagList.push([x]);
                                            break;
                                        case "BITRATE":
                                            k.tagList.push([x, P]);
                                            break;
                                        case "DISCONTINUITY-SEQ":
                                            A = parseInt(P);
                                            break;
                                        case "KEY":
                                            var G,
                                                K = new v(P),
                                                j = K.enumeratedString("METHOD"),
                                                H = K.URI,
                                                V = K.hexadecimalInteger("IV"),
                                                W = K.enumeratedString("KEYFORMATVERSIONS"),
                                                Y = K.enumeratedString("KEYID"),
                                                q = null != (G = K.enumeratedString("KEYFORMAT")) ? G : "identity";
                                            if (["com.apple.streamingkeydelivery", "com.microsoft.playready", "urn:uuid:edef8ba9-79d6-4ace-a3c8-27dcd51d21ed", "com.widevine"].indexOf(q) > -1) {
                                                o.b.warn("Keyformat " + q + " is not supported from the manifest");
                                                continue;
                                            }
                                            if ("identity" !== q) continue;
                                            j &&
                                                ((f = c.a.fromURL(e, H)),
                                                H && ["AES-128", "SAMPLE-AES", "SAMPLE-AES-CENC"].indexOf(j) >= 0 && ((f.method = j), (f.keyFormat = q), Y && (f.keyID = Y), W && (f.keyFormatVersions = W), (f.iv = V)));
                                            break;
                                        case "START":
                                            var X = new v(P).decimalFloatingPoint("TIME-OFFSET");
                                            Object(n.a)(X) && (g.startTimeOffset = X);
                                            break;
                                        case "MAP":
                                            var z = new v(P);
                                            (k.relurl = z.URI), z.BYTERANGE && k.setByteRange(z.BYTERANGE), (k.level = r), (k.sn = "initSegment"), f && (k.levelkey = f), (k.initSegment = null), (m = k), (I = !0);
                                            break;
                                        case "SERVER-CONTROL":
                                            var Q = new v(P);
                                            (g.canBlockReload = Q.bool("CAN-BLOCK-RELOAD")),
                                                (g.canSkipUntil = Q.optionalFloat("CAN-SKIP-UNTIL", 0)),
                                                (g.canSkipDateRanges = g.canSkipUntil > 0 && Q.bool("CAN-SKIP-DATERANGES")),
                                                (g.partHoldBack = Q.optionalFloat("PART-HOLD-BACK", 0)),
                                                (g.holdBack = Q.optionalFloat("HOLD-BACK", 0));
                                            break;
                                        case "PART-INF":
                                            var $ = new v(P);
                                            g.partTarget = $.decimalFloatingPoint("PART-TARGET");
                                            break;
                                        case "PART":
                                            var J = g.partList;
                                            J || (J = g.partList = []);
                                            var Z = T > 0 ? J[J.length - 1] : void 0,
                                                tt = T++,
                                                et = new u.c(new v(P), k, e, tt, Z);
                                            J.push(et), (k.duration += et.duration);
                                            break;
                                        case "PRELOAD-HINT":
                                            var rt = new v(P);
                                            g.preloadHint = rt;
                                            break;
                                        case "RENDITION-REPORT":
                                            var it = new v(P);
                                            (g.renditionReports = g.renditionReports || []), g.renditionReports.push(it);
                                            break;
                                        default:
                                            o.b.warn("line parsed but not handled: " + l);
                                    }
                                }
                            }
                            R && !R.relurl ? (p.pop(), (L -= R.duration), g.partList && (g.fragmentHint = R)) : g.partList && (D(k, R), (k.cc = A), (g.fragmentHint = k));
                            var nt = p.length,
                                at = p[0],
                                st = p[nt - 1];
                            if ((L += g.skippedSegments * g.targetduration) > 0 && nt && st) {
                                g.averagetargetduration = L / nt;
                                var ot = st.sn;
                                (g.endSN = "initSegment" !== ot ? ot : 0),
                                    at &&
                                        ((g.startCC = at.cc),
                                        at.initSegment ||
                                            (g.fragments.every(function (t) {
                                                return t.relurl && ((e = t.relurl), S.test(null != (r = null === (n = i.parseURL(e)) || void 0 === n ? void 0 : n.path) ? r : ""));
                                                var e, r, n;
                                            }) &&
                                                (o.b.warn("MP4 fragments found but no init segment (probably no MAP, incomplete M3U8), trying to fetch SIDX"),
                                                ((k = new u.b(a, e)).relurl = st.relurl),
                                                (k.level = r),
                                                (k.sn = "initSegment"),
                                                (at.initSegment = k),
                                                (g.needSidxRanges = !0))));
                            } else (g.endSN = 0), (g.startCC = 0);
                            return (
                                g.fragmentHint && (L += g.fragmentHint.duration),
                                (g.totalduration = L),
                                (g.endCC = A),
                                _ > 0 &&
                                    (function (t, e) {
                                        for (var r = t[e], i = e; i--; ) {
                                            var n = t[i];
                                            if (!n) return;
                                            (n.programDateTime = r.programDateTime - 1e3 * n.duration), (r = n);
                                        }
                                    })(p, _),
                                g
                            );
                        }),
                        t
                    );
                })();
                function A(t, e) {
                    ["video", "audio", "text"].forEach(function (r) {
                        var i = t.filter(function (t) {
                            return (function (t, e) {
                                var r = p[e];
                                return !!r && !0 === r[t.slice(0, 4)];
                            })(t, r);
                        });
                        if (i.length) {
                            var n = i.filter(function (t) {
                                return 0 === t.lastIndexOf("avc1", 0) || 0 === t.lastIndexOf("mp4a", 0);
                            });
                            (e[r + "Codec"] = n.length > 0 ? n[0] : i[0]),
                                (t = t.filter(function (t) {
                                    return -1 === i.indexOf(t);
                                }));
                        }
                    }),
                        (e.unknownCodecs = t);
                }
                function R(t, e, r) {
                    var i = e[r];
                    i && (t[r] = i);
                }
                function D(t, e) {
                    t.rawProgramDateTime ? (t.programDateTime = Date.parse(t.rawProgramDateTime)) : null != e && e.programDateTime && (t.programDateTime = e.endProgramDateTime),
                        Object(n.a)(t.programDateTime) || ((t.programDateTime = null), (t.rawProgramDateTime = null));
                }
                var k = r(4);
                function _(t, e) {
                    var r = t.url;
                    return (void 0 !== r && 0 !== r.indexOf("data:")) || (r = e.url), r;
                }
                var I = (function () {
                        function t(t) {
                            (this.hls = void 0), (this.loaders = Object.create(null)), (this.hls = t), this.registerListeners();
                        }
                        var e = t.prototype;
                        return (
                            (e.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.on(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                    t.on(a.a.AUDIO_TRACK_LOADING, this.onAudioTrackLoading, this),
                                    t.on(a.a.SUBTITLE_TRACK_LOADING, this.onSubtitleTrackLoading, this);
                            }),
                            (e.unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.off(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                    t.off(a.a.AUDIO_TRACK_LOADING, this.onAudioTrackLoading, this),
                                    t.off(a.a.SUBTITLE_TRACK_LOADING, this.onSubtitleTrackLoading, this);
                            }),
                            (e.createInternalLoader = function (t) {
                                var e = this.hls.config,
                                    r = e.pLoader,
                                    i = e.loader,
                                    n = new (r || i)(e);
                                return (t.loader = n), (this.loaders[t.type] = n), n;
                            }),
                            (e.getInternalLoader = function (t) {
                                return this.loaders[t.type];
                            }),
                            (e.resetInternalLoader = function (t) {
                                this.loaders[t] && delete this.loaders[t];
                            }),
                            (e.destroyInternalLoaders = function () {
                                for (var t in this.loaders) {
                                    var e = this.loaders[t];
                                    e && e.destroy(), this.resetInternalLoader(t);
                                }
                            }),
                            (e.destroy = function () {
                                this.unregisterListeners(), this.destroyInternalLoaders();
                            }),
                            (e.onManifestLoading = function (t, e) {
                                var r = e.url;
                                this.load({ id: null, groupId: null, level: 0, responseType: "text", type: k.a.MANIFEST, url: r, deliveryDirectives: null });
                            }),
                            (e.onLevelLoading = function (t, e) {
                                var r = e.id,
                                    i = e.level,
                                    n = e.url,
                                    a = e.deliveryDirectives;
                                this.load({ id: r, groupId: null, level: i, responseType: "text", type: k.a.LEVEL, url: n, deliveryDirectives: a });
                            }),
                            (e.onAudioTrackLoading = function (t, e) {
                                var r = e.id,
                                    i = e.groupId,
                                    n = e.url,
                                    a = e.deliveryDirectives;
                                this.load({ id: r, groupId: i, level: null, responseType: "text", type: k.a.AUDIO_TRACK, url: n, deliveryDirectives: a });
                            }),
                            (e.onSubtitleTrackLoading = function (t, e) {
                                var r = e.id,
                                    i = e.groupId,
                                    n = e.url,
                                    a = e.deliveryDirectives;
                                this.load({ id: r, groupId: i, level: null, responseType: "text", type: k.a.SUBTITLE_TRACK, url: n, deliveryDirectives: a });
                            }),
                            (e.load = function (t) {
                                var e,
                                    r,
                                    i,
                                    n,
                                    a,
                                    s,
                                    l = this.hls.config,
                                    u = this.getInternalLoader(t);
                                if (u) {
                                    var h = u.context;
                                    if (h && h.url === t.url) return void o.b.trace("[playlist-loader]: playlist request ongoing");
                                    o.b.log("[playlist-loader]: aborting previous loader for type: " + t.type), u.abort();
                                }
                                switch (t.type) {
                                    case k.a.MANIFEST:
                                        (r = l.manifestLoadingMaxRetry), (i = l.manifestLoadingTimeOut), (n = l.manifestLoadingRetryDelay), (a = l.manifestLoadingMaxRetryTimeout);
                                        break;
                                    case k.a.LEVEL:
                                    case k.a.AUDIO_TRACK:
                                    case k.a.SUBTITLE_TRACK:
                                        (r = 0), (i = l.levelLoadingTimeOut);
                                        break;
                                    default:
                                        (r = l.levelLoadingMaxRetry), (i = l.levelLoadingTimeOut), (n = l.levelLoadingRetryDelay), (a = l.levelLoadingMaxRetryTimeout);
                                }
                                if (
                                    ((u = this.createInternalLoader(t)), null !== (e = t.deliveryDirectives) && void 0 !== e && e.part) &&
                                    (t.type === k.a.LEVEL && null !== t.level
                                        ? (s = this.hls.levels[t.level].details)
                                        : t.type === k.a.AUDIO_TRACK && null !== t.id
                                        ? (s = this.hls.audioTracks[t.id].details)
                                        : t.type === k.a.SUBTITLE_TRACK && null !== t.id && (s = this.hls.subtitleTracks[t.id].details),
                                    s)
                                ) {
                                    var d = s.partTarget,
                                        c = s.targetduration;
                                    d && c && (i = Math.min(1e3 * Math.max(3 * d, 0.8 * c), i));
                                }
                                var f = { timeout: i, maxRetry: r, retryDelay: n, maxRetryDelay: a, highWaterMark: 0 },
                                    g = { onSuccess: this.loadsuccess.bind(this), onError: this.loaderror.bind(this), onTimeout: this.loadtimeout.bind(this) };
                                u.load(t, f, g);
                            }),
                            (e.loadsuccess = function (t, e, r, i) {
                                if ((void 0 === i && (i = null), r.isSidxRequest)) return this.handleSidxRequest(t, r), void this.handlePlaylistLoaded(t, e, r, i);
                                this.resetInternalLoader(r.type);
                                var n = t.data;
                                0 === n.indexOf("#EXTM3U")
                                    ? ((e.parsing.start = performance.now()), n.indexOf("#EXTINF:") > 0 || n.indexOf("#EXT-X-TARGETDURATION:") > 0 ? this.handleTrackOrLevelPlaylist(t, e, r, i) : this.handleMasterPlaylist(t, e, r, i))
                                    : this.handleManifestParsingError(t, r, "no EXTM3U delimiter", i);
                            }),
                            (e.loaderror = function (t, e, r) {
                                void 0 === r && (r = null), this.handleNetworkError(e, r, !1, t);
                            }),
                            (e.loadtimeout = function (t, e, r) {
                                void 0 === r && (r = null), this.handleNetworkError(e, r, !0);
                            }),
                            (e.handleMasterPlaylist = function (t, e, r, i) {
                                var n = this.hls,
                                    s = t.data,
                                    l = _(t, r),
                                    u = L.parseMasterPlaylist(s, l),
                                    h = u.levels,
                                    d = u.sessionData;
                                if (h.length) {
                                    var c = h.map(function (t) {
                                            return { id: t.attrs.AUDIO, audioCodec: t.audioCodec };
                                        }),
                                        f = h.map(function (t) {
                                            return { id: t.attrs.SUBTITLES, textCodec: t.textCodec };
                                        }),
                                        g = L.parseMasterPlaylistMedia(s, l, "AUDIO", c),
                                        p = L.parseMasterPlaylistMedia(s, l, "SUBTITLES", f),
                                        m = L.parseMasterPlaylistMedia(s, l, "CLOSED-CAPTIONS");
                                    if (g.length)
                                        g.some(function (t) {
                                            return !t.url;
                                        }) ||
                                            !h[0].audioCodec ||
                                            h[0].attrs.AUDIO ||
                                            (o.b.log("[playlist-loader]: audio codec signaled in quality level, but no embedded audio track signaled, create one"),
                                            g.unshift({ type: "main", name: "main", default: !1, autoselect: !1, forced: !1, id: -1, attrs: new v({}), bitrate: 0, url: "" }));
                                    n.trigger(a.a.MANIFEST_LOADED, { levels: h, audioTracks: g, subtitles: p, captions: m, url: l, stats: e, networkDetails: i, sessionData: d });
                                } else this.handleManifestParsingError(t, r, "no level found in manifest", i);
                            }),
                            (e.handleTrackOrLevelPlaylist = function (t, e, r, i) {
                                var o = this.hls,
                                    l = r.id,
                                    u = r.level,
                                    h = r.type,
                                    d = _(t, r),
                                    c = Object(n.a)(l) ? l : 0,
                                    f = Object(n.a)(u) ? u : c,
                                    g = (function (t) {
                                        switch (t.type) {
                                            case k.a.AUDIO_TRACK:
                                                return k.b.AUDIO;
                                            case k.a.SUBTITLE_TRACK:
                                                return k.b.SUBTITLE;
                                            default:
                                                return k.b.MAIN;
                                        }
                                    })(r),
                                    p = L.parseLevelPlaylist(t.data, d, f, g, c);
                                if (p.fragments.length) {
                                    if (h === k.a.MANIFEST) {
                                        var m = { attrs: new v({}), bitrate: 0, details: p, name: "", url: d };
                                        o.trigger(a.a.MANIFEST_LOADED, { levels: [m], audioTracks: [], url: d, stats: e, networkDetails: i, sessionData: null });
                                    }
                                    if (((e.parsing.end = performance.now()), p.needSidxRanges)) {
                                        var y,
                                            T = null === (y = p.fragments[0].initSegment) || void 0 === y ? void 0 : y.url;
                                        this.load({ url: T, isSidxRequest: !0, type: h, level: u, levelDetails: p, id: l, groupId: null, rangeStart: 0, rangeEnd: 2048, responseType: "arraybuffer", deliveryDirectives: null });
                                    } else (r.levelDetails = p), this.handlePlaylistLoaded(t, e, r, i);
                                } else o.trigger(a.a.ERROR, { type: s.b.NETWORK_ERROR, details: s.a.LEVEL_EMPTY_ERROR, fatal: !1, url: d, reason: "no fragments found in level", level: "number" == typeof r.level ? r.level : void 0 });
                            }),
                            (e.handleSidxRequest = function (t, e) {
                                var r = Object(l.g)(new Uint8Array(t.data));
                                if (r) {
                                    var i = r.references,
                                        n = e.levelDetails;
                                    i.forEach(function (t, e) {
                                        var i = t.info,
                                            a = n.fragments[e];
                                        0 === a.byteRange.length && a.setByteRange(String(1 + i.end - i.start) + "@" + String(i.start)), a.initSegment && a.initSegment.setByteRange(String(r.moovEndOffset) + "@0");
                                    });
                                }
                            }),
                            (e.handleManifestParsingError = function (t, e, r, i) {
                                this.hls.trigger(a.a.ERROR, { type: s.b.NETWORK_ERROR, details: s.a.MANIFEST_PARSING_ERROR, fatal: e.type === k.a.MANIFEST, url: t.url, reason: r, response: t, context: e, networkDetails: i });
                            }),
                            (e.handleNetworkError = function (t, e, r, i) {
                                void 0 === r && (r = !1),
                                    o.b.warn("[playlist-loader]: A network " + (r ? "timeout" : "error") + " occurred while loading " + t.type + " level: " + t.level + " id: " + t.id + ' group-id: "' + t.groupId + '"');
                                var n = s.a.UNKNOWN,
                                    l = !1,
                                    u = this.getInternalLoader(t);
                                switch (t.type) {
                                    case k.a.MANIFEST:
                                        (n = r ? s.a.MANIFEST_LOAD_TIMEOUT : s.a.MANIFEST_LOAD_ERROR), (l = !0);
                                        break;
                                    case k.a.LEVEL:
                                        (n = r ? s.a.LEVEL_LOAD_TIMEOUT : s.a.LEVEL_LOAD_ERROR), (l = !1);
                                        break;
                                    case k.a.AUDIO_TRACK:
                                        (n = r ? s.a.AUDIO_TRACK_LOAD_TIMEOUT : s.a.AUDIO_TRACK_LOAD_ERROR), (l = !1);
                                        break;
                                    case k.a.SUBTITLE_TRACK:
                                        (n = r ? s.a.SUBTITLE_TRACK_LOAD_TIMEOUT : s.a.SUBTITLE_LOAD_ERROR), (l = !1);
                                }
                                u && this.resetInternalLoader(t.type);
                                var h = { type: s.b.NETWORK_ERROR, details: n, fatal: l, url: t.url, loader: u, context: t, networkDetails: e };
                                i && (h.response = i), this.hls.trigger(a.a.ERROR, h);
                            }),
                            (e.handlePlaylistLoaded = function (t, e, r, i) {
                                var n = r.type,
                                    s = r.level,
                                    o = r.id,
                                    l = r.groupId,
                                    u = r.loader,
                                    h = r.levelDetails,
                                    d = r.deliveryDirectives;
                                if (null != h && h.targetduration) {
                                    if (u)
                                        switch ((h.live && (u.getCacheAge && (h.ageHeader = u.getCacheAge() || 0), (u.getCacheAge && !isNaN(h.ageHeader)) || (h.ageHeader = 0)), n)) {
                                            case k.a.MANIFEST:
                                            case k.a.LEVEL:
                                                this.hls.trigger(a.a.LEVEL_LOADED, { details: h, level: s || 0, id: o || 0, stats: e, networkDetails: i, deliveryDirectives: d });
                                                break;
                                            case k.a.AUDIO_TRACK:
                                                this.hls.trigger(a.a.AUDIO_TRACK_LOADED, { details: h, id: o || 0, groupId: l || "", stats: e, networkDetails: i, deliveryDirectives: d });
                                                break;
                                            case k.a.SUBTITLE_TRACK:
                                                this.hls.trigger(a.a.SUBTITLE_TRACK_LOADED, { details: h, id: o || 0, groupId: l || "", stats: e, networkDetails: i, deliveryDirectives: d });
                                        }
                                } else this.handleManifestParsingError(t, r, "invalid target duration", i);
                            }),
                            t
                        );
                    })(),
                    C = (function () {
                        function t(t) {
                            (this.hls = void 0), (this.loaders = {}), (this.decryptkey = null), (this.decrypturl = null), (this.hls = t), this._registerListeners();
                        }
                        var e = t.prototype;
                        return (
                            (e._registerListeners = function () {
                                this.hls.on(a.a.KEY_LOADING, this.onKeyLoading, this);
                            }),
                            (e._unregisterListeners = function () {
                                this.hls.off(a.a.KEY_LOADING, this.onKeyLoading);
                            }),
                            (e.destroy = function () {
                                for (var t in (this._unregisterListeners(), this.loaders)) {
                                    var e = this.loaders[t];
                                    e && e.destroy();
                                }
                                this.loaders = {};
                            }),
                            (e.onKeyLoading = function (t, e) {
                                var r = e.frag,
                                    i = r.type,
                                    n = this.loaders[i];
                                if (r.decryptdata) {
                                    var s = r.decryptdata.uri;
                                    if (s !== this.decrypturl || null === this.decryptkey) {
                                        var l = this.hls.config;
                                        if ((n && (o.b.warn("abort previous key loader for type:" + i), n.abort()), !s)) return void o.b.warn("key uri is falsy");
                                        var u = l.loader,
                                            h = (r.loader = this.loaders[i] = new u(l));
                                        (this.decrypturl = s), (this.decryptkey = null);
                                        var d = { url: s, frag: r, responseType: "arraybuffer" },
                                            c = { timeout: l.fragLoadingTimeOut, maxRetry: 0, retryDelay: l.fragLoadingRetryDelay, maxRetryDelay: l.fragLoadingMaxRetryTimeout, highWaterMark: 0 },
                                            f = { onSuccess: this.loadsuccess.bind(this), onError: this.loaderror.bind(this), onTimeout: this.loadtimeout.bind(this) };
                                        h.load(d, c, f);
                                    } else this.decryptkey && ((r.decryptdata.key = this.decryptkey), this.hls.trigger(a.a.KEY_LOADED, { frag: r }));
                                } else o.b.warn("Missing decryption data on fragment in onKeyLoading");
                            }),
                            (e.loadsuccess = function (t, e, r) {
                                var i = r.frag;
                                i.decryptdata
                                    ? ((this.decryptkey = i.decryptdata.key = new Uint8Array(t.data)), (i.loader = null), delete this.loaders[i.type], this.hls.trigger(a.a.KEY_LOADED, { frag: i }))
                                    : o.b.error("after key load, decryptdata unset");
                            }),
                            (e.loaderror = function (t, e) {
                                var r = e.frag,
                                    i = r.loader;
                                i && i.abort(), delete this.loaders[r.type], this.hls.trigger(a.a.ERROR, { type: s.b.NETWORK_ERROR, details: s.a.KEY_LOAD_ERROR, fatal: !1, frag: r, response: t });
                            }),
                            (e.loadtimeout = function (t, e) {
                                var r = e.frag,
                                    i = r.loader;
                                i && i.abort(), delete this.loaders[r.type], this.hls.trigger(a.a.ERROR, { type: s.b.NETWORK_ERROR, details: s.a.KEY_LOAD_TIMEOUT, fatal: !1, frag: r });
                            }),
                            t
                        );
                    })();
                function w(t, e) {
                    var r;
                    try {
                        r = new Event("addtrack");
                    } catch (t) {
                        (r = document.createEvent("Event")).initEvent("addtrack", !1, !1);
                    }
                    (r.track = t), e.dispatchEvent(r);
                }
                function O(t, e) {
                    var r = t.mode;
                    if (("disabled" === r && (t.mode = "hidden"), t.cues && !t.cues.getCueById(e.id)))
                        try {
                            if ((t.addCue(e), !t.cues.getCueById(e.id))) throw new Error("addCue is failed for: " + e);
                        } catch (r) {
                            o.b.debug("[texttrack-utils]: " + r);
                            var i = new self.TextTrackCue(e.startTime, e.endTime, e.text);
                            (i.id = e.id), t.addCue(i);
                        }
                    "disabled" === r && (t.mode = r);
                }
                function x(t) {
                    var e = t.mode;
                    if (("disabled" === e && (t.mode = "hidden"), t.cues)) for (var r = t.cues.length; r--; ) t.removeCue(t.cues[r]);
                    "disabled" === e && (t.mode = e);
                }
                function P(t, e, r) {
                    var i = t.mode;
                    if (("disabled" === i && (t.mode = "hidden"), t.cues && t.cues.length > 0))
                        for (
                            var n = (function (t, e, r) {
                                    var i = [],
                                        n = (function (t, e) {
                                            if (e < t[0].startTime) return 0;
                                            var r = t.length - 1;
                                            if (e > t[r].endTime) return -1;
                                            var i = 0,
                                                n = r;
                                            for (; i <= n; ) {
                                                var a = Math.floor((n + i) / 2);
                                                if (e < t[a].startTime) n = a - 1;
                                                else {
                                                    if (!(e > t[a].startTime && i < r)) return a;
                                                    i = a + 1;
                                                }
                                            }
                                            return t[i].startTime - e < e - t[n].startTime ? i : n;
                                        })(t, e);
                                    if (n > -1)
                                        for (var a = n, s = t.length; a < s; a++) {
                                            var o = t[a];
                                            if (o.startTime >= e && o.endTime <= r) i.push(o);
                                            else if (o.startTime > r) return i;
                                        }
                                    return i;
                                })(t.cues, e, r),
                                a = 0;
                            a < n.length;
                            a++
                        )
                            t.removeCue(n[a]);
                    "disabled" === i && (t.mode = i);
                }
                var M = r(7),
                    F = (function () {
                        function t(t) {
                            (this.hls = void 0), (this.id3Track = null), (this.media = null), (this.hls = t), this._registerListeners();
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                this._unregisterListeners();
                            }),
                            (e._registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.on(a.a.FRAG_PARSING_METADATA, this.onFragParsingMetadata, this),
                                    t.on(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this);
                            }),
                            (e._unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.off(a.a.FRAG_PARSING_METADATA, this.onFragParsingMetadata, this),
                                    t.off(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this);
                            }),
                            (e.onMediaAttached = function (t, e) {
                                this.media = e.media;
                            }),
                            (e.onMediaDetaching = function () {
                                this.id3Track && (x(this.id3Track), (this.id3Track = null), (this.media = null));
                            }),
                            (e.getID3Track = function (t) {
                                if (this.media) {
                                    for (var e = 0; e < t.length; e++) {
                                        var r = t[e];
                                        if ("metadata" === r.kind && "id3" === r.label) return w(r, this.media), r;
                                    }
                                    return this.media.addTextTrack("metadata", "id3");
                                }
                            }),
                            (e.onFragParsingMetadata = function (t, e) {
                                if (this.media) {
                                    var r = e.frag,
                                        i = e.samples;
                                    this.id3Track || ((this.id3Track = this.getID3Track(this.media.textTracks)), (this.id3Track.mode = "hidden"));
                                    for (var n = self.WebKitDataCue || self.VTTCue || self.TextTrackCue, a = 0; a < i.length; a++) {
                                        var s = M.c(i[a].data);
                                        if (s) {
                                            var o = i[a].pts,
                                                l = a < i.length - 1 ? i[a + 1].pts : r.end;
                                            l - o <= 0 && (l = o + 0.25);
                                            for (var u = 0; u < s.length; u++) {
                                                var h = s[u];
                                                if (!M.e(h)) {
                                                    var d = new n(o, l, "");
                                                    (d.value = h), this.id3Track.addCue(d);
                                                }
                                            }
                                        }
                                    }
                                }
                            }),
                            (e.onBufferFlushing = function (t, e) {
                                var r = e.startOffset,
                                    i = e.endOffset,
                                    n = e.type;
                                if (!n || "audio" === n) {
                                    var a = this.id3Track;
                                    a && P(a, r, i);
                                }
                            }),
                            t
                        );
                    })();
                function N(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var U,
                    B = (function () {
                        function t(t) {
                            var e = this;
                            (this.hls = void 0),
                                (this.config = void 0),
                                (this.media = null),
                                (this.levelDetails = null),
                                (this.currentTime = 0),
                                (this.stallCount = 0),
                                (this._latency = null),
                                (this.timeupdateHandler = function () {
                                    return e.timeupdate();
                                }),
                                (this.hls = t),
                                (this.config = t.config),
                                this.registerListeners();
                        }
                        var e,
                            r,
                            i,
                            n = t.prototype;
                        return (
                            (n.destroy = function () {
                                this.unregisterListeners(), this.onMediaDetaching(), (this.levelDetails = null), (this.hls = this.timeupdateHandler = null);
                            }),
                            (n.registerListeners = function () {
                                this.hls.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    this.hls.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    this.hls.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    this.hls.on(a.a.LEVEL_UPDATED, this.onLevelUpdated, this),
                                    this.hls.on(a.a.ERROR, this.onError, this);
                            }),
                            (n.unregisterListeners = function () {
                                this.hls.off(a.a.MEDIA_ATTACHED, this.onMediaAttached),
                                    this.hls.off(a.a.MEDIA_DETACHING, this.onMediaDetaching),
                                    this.hls.off(a.a.MANIFEST_LOADING, this.onManifestLoading),
                                    this.hls.off(a.a.LEVEL_UPDATED, this.onLevelUpdated),
                                    this.hls.off(a.a.ERROR, this.onError);
                            }),
                            (n.onMediaAttached = function (t, e) {
                                (this.media = e.media), this.media.addEventListener("timeupdate", this.timeupdateHandler);
                            }),
                            (n.onMediaDetaching = function () {
                                this.media && (this.media.removeEventListener("timeupdate", this.timeupdateHandler), (this.media = null));
                            }),
                            (n.onManifestLoading = function () {
                                (this.levelDetails = null), (this._latency = null), (this.stallCount = 0);
                            }),
                            (n.onLevelUpdated = function (t, e) {
                                var r = e.details;
                                (this.levelDetails = r), r.advanced && this.timeupdate(), !r.live && this.media && this.media.removeEventListener("timeupdate", this.timeupdateHandler);
                            }),
                            (n.onError = function (t, e) {
                                e.details === s.a.BUFFER_STALLED_ERROR && (this.stallCount++, o.b.warn("[playback-rate-controller]: Stall detected, adjusting target latency"));
                            }),
                            (n.timeupdate = function () {
                                var t = this.media,
                                    e = this.levelDetails;
                                if (t && e) {
                                    this.currentTime = t.currentTime;
                                    var r = this.computeLatency();
                                    if (null !== r) {
                                        this._latency = r;
                                        var i = this.config,
                                            n = i.lowLatencyMode,
                                            a = i.maxLiveSyncPlaybackRate;
                                        if (n && 1 !== a) {
                                            var s = this.targetLatency;
                                            if (null !== s) {
                                                var o = r - s,
                                                    l = o < Math.min(this.maxLatency, s + e.targetduration);
                                                if (e.live && l && o > 0.05 && this.forwardBufferLength > 1) {
                                                    var u = Math.min(2, Math.max(1, a)),
                                                        h = Math.round((2 / (1 + Math.exp(-0.75 * o - this.edgeStalled))) * 20) / 20;
                                                    t.playbackRate = Math.min(u, Math.max(1, h));
                                                } else 1 !== t.playbackRate && 0 !== t.playbackRate && (t.playbackRate = 1);
                                            }
                                        }
                                    }
                                }
                            }),
                            (n.estimateLiveEdge = function () {
                                var t = this.levelDetails;
                                return null === t ? null : t.edge + t.age;
                            }),
                            (n.computeLatency = function () {
                                var t = this.estimateLiveEdge();
                                return null === t ? null : t - this.currentTime;
                            }),
                            (e = t),
                            (r = [
                                {
                                    key: "latency",
                                    get: function () {
                                        return this._latency || 0;
                                    },
                                },
                                {
                                    key: "maxLatency",
                                    get: function () {
                                        var t = this.config,
                                            e = this.levelDetails;
                                        return void 0 !== t.liveMaxLatencyDuration ? t.liveMaxLatencyDuration : e ? t.liveMaxLatencyDurationCount * e.targetduration : 0;
                                    },
                                },
                                {
                                    key: "targetLatency",
                                    get: function () {
                                        var t = this.levelDetails;
                                        if (null === t) return null;
                                        var e = t.holdBack,
                                            r = t.partHoldBack,
                                            i = t.targetduration,
                                            n = this.config,
                                            a = n.liveSyncDuration,
                                            s = n.liveSyncDurationCount,
                                            o = n.lowLatencyMode,
                                            l = this.hls.userConfig,
                                            u = (o && r) || e;
                                        (l.liveSyncDuration || l.liveSyncDurationCount || 0 === u) && (u = void 0 !== a ? a : s * i);
                                        var h = i;
                                        return u + Math.min(1 * this.stallCount, h);
                                    },
                                },
                                {
                                    key: "liveSyncPosition",
                                    get: function () {
                                        var t = this.estimateLiveEdge(),
                                            e = this.targetLatency,
                                            r = this.levelDetails;
                                        if (null === t || null === e || null === r) return null;
                                        var i = r.edge,
                                            n = t - e - this.edgeStalled,
                                            a = i - r.totalduration,
                                            s = i - ((this.config.lowLatencyMode && r.partTarget) || r.targetduration);
                                        return Math.min(Math.max(a, n), s);
                                    },
                                },
                                {
                                    key: "drift",
                                    get: function () {
                                        var t = this.levelDetails;
                                        return null === t ? 1 : t.drift;
                                    },
                                },
                                {
                                    key: "edgeStalled",
                                    get: function () {
                                        var t = this.levelDetails;
                                        if (null === t) return 0;
                                        var e = 3 * ((this.config.lowLatencyMode && t.partTarget) || t.targetduration);
                                        return Math.max(t.age - e, 0);
                                    },
                                },
                                {
                                    key: "forwardBufferLength",
                                    get: function () {
                                        var t = this.media,
                                            e = this.levelDetails;
                                        if (!t || !e) return 0;
                                        var r = t.buffered.length;
                                        return r ? t.buffered.end(r - 1) : e.edge - this.currentTime;
                                    },
                                },
                            ]) && N(e.prototype, r),
                            i && N(e, i),
                            t
                        );
                    })();
                function G(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                !(function (t) {
                    (t.No = ""), (t.Yes = "YES"), (t.v2 = "v2");
                })(U || (U = {}));
                var K = (function () {
                        function t(t, e, r) {
                            (this.msn = void 0), (this.part = void 0), (this.skip = void 0), (this.msn = t), (this.part = e), (this.skip = r);
                        }
                        return (
                            (t.prototype.addDirectives = function (t) {
                                var e = new self.URL(t);
                                return (
                                    void 0 !== this.msn && e.searchParams.set("_HLS_msn", this.msn.toString()),
                                    void 0 !== this.part && e.searchParams.set("_HLS_part", this.part.toString()),
                                    this.skip && e.searchParams.set("_HLS_skip", this.skip),
                                    e.toString()
                                );
                            }),
                            t
                        );
                    })(),
                    j = (function () {
                        function t(t) {
                            (this.attrs = void 0),
                                (this.audioCodec = void 0),
                                (this.bitrate = void 0),
                                (this.codecSet = void 0),
                                (this.height = void 0),
                                (this.id = void 0),
                                (this.name = void 0),
                                (this.videoCodec = void 0),
                                (this.width = void 0),
                                (this.unknownCodecs = void 0),
                                (this.audioGroupIds = void 0),
                                (this.details = void 0),
                                (this.fragmentError = 0),
                                (this.loadError = 0),
                                (this.loaded = void 0),
                                (this.realBitrate = 0),
                                (this.textGroupIds = void 0),
                                (this.url = void 0),
                                (this._urlId = 0),
                                (this.url = [t.url]),
                                (this.attrs = t.attrs),
                                (this.bitrate = t.bitrate),
                                t.details && (this.details = t.details),
                                (this.id = t.id || 0),
                                (this.name = t.name),
                                (this.width = t.width || 0),
                                (this.height = t.height || 0),
                                (this.audioCodec = t.audioCodec),
                                (this.videoCodec = t.videoCodec),
                                (this.unknownCodecs = t.unknownCodecs),
                                (this.codecSet = [t.videoCodec, t.audioCodec]
                                    .filter(function (t) {
                                        return t;
                                    })
                                    .join(",")
                                    .replace(/\.[^.,]+/g, ""));
                        }
                        var e, r, i;
                        return (
                            (e = t),
                            (r = [
                                {
                                    key: "maxBitrate",
                                    get: function () {
                                        return Math.max(this.realBitrate, this.bitrate);
                                    },
                                },
                                {
                                    key: "uri",
                                    get: function () {
                                        return this.url[this._urlId] || "";
                                    },
                                },
                                {
                                    key: "urlId",
                                    get: function () {
                                        return this._urlId;
                                    },
                                    set: function (t) {
                                        var e = t % this.url.length;
                                        this._urlId !== e && ((this.details = void 0), (this._urlId = e));
                                    },
                                },
                            ]) && G(e.prototype, r),
                            i && G(e, i),
                            t
                        );
                    })();
                function H(t, e, r) {
                    switch (e) {
                        case "audio":
                            t.audioGroupIds || (t.audioGroupIds = []), t.audioGroupIds.push(r);
                            break;
                        case "text":
                            t.textGroupIds || (t.textGroupIds = []), t.textGroupIds.push(r);
                    }
                }
                function V(t) {
                    var e = {};
                    t.forEach(function (t) {
                        var r = t.groupId || "";
                        (t.id = e[r] = e[r] || 0), e[r]++;
                    });
                }
                function W(t, e) {
                    var r = e.startPTS;
                    if (Object(n.a)(r)) {
                        var i,
                            a = 0;
                        e.sn > t.sn ? ((a = r - t.start), (i = t)) : ((a = t.start - r), (i = e)), i.duration !== a && (i.duration = a);
                    } else if (e.sn > t.sn) {
                        t.cc === e.cc && t.minEndPTS ? (e.start = t.start + (t.minEndPTS - t.start)) : (e.start = t.start + t.duration);
                    } else e.start = Math.max(t.start - e.duration, 0);
                }
                function Y(t, e, r, i, a, s) {
                    i - r <= 0 && (o.b.warn("Fragment should have a positive duration", e), (i = r + e.duration), (s = a + e.duration));
                    var l = r,
                        u = i,
                        h = e.startPTS,
                        d = e.endPTS;
                    if (Object(n.a)(h)) {
                        var c = Math.abs(h - r);
                        Object(n.a)(e.deltaPTS) ? (e.deltaPTS = Math.max(c, e.deltaPTS)) : (e.deltaPTS = c),
                            (l = Math.max(r, h)),
                            (r = Math.min(r, h)),
                            (a = Math.min(a, e.startDTS)),
                            (u = Math.min(i, d)),
                            (i = Math.max(i, d)),
                            (s = Math.max(s, e.endDTS));
                    }
                    e.duration = i - r;
                    var f = r - e.start;
                    (e.appendedPTS = i), (e.start = e.startPTS = r), (e.maxStartPTS = l), (e.startDTS = a), (e.endPTS = i), (e.minEndPTS = u), (e.endDTS = s);
                    var g,
                        v = e.sn;
                    if (!t || v < t.startSN || v > t.endSN) return 0;
                    var p = v - t.startSN,
                        m = t.fragments;
                    for (m[p] = e, g = p; g > 0; g--) W(m[g], m[g - 1]);
                    for (g = p; g < m.length - 1; g++) W(m[g], m[g + 1]);
                    return t.fragmentHint && W(m[m.length - 1], t.fragmentHint), (t.PTSKnown = t.alignedSliding = !0), f;
                }
                function q(t, e) {
                    for (var r = null, i = t.fragments, a = i.length - 1; a >= 0; a--) {
                        var s = i[a].initSegment;
                        if (s) {
                            r = s;
                            break;
                        }
                    }
                    t.fragmentHint && delete t.fragmentHint.endPTS;
                    var l,
                        u = 0;
                    if (
                        ((function (t, e, r) {
                            for (
                                var i = e.skippedSegments,
                                    n = Math.max(t.startSN, e.startSN) - e.startSN,
                                    a = (t.fragmentHint ? 1 : 0) + (i ? e.endSN : Math.min(t.endSN, e.endSN)) - e.startSN,
                                    s = e.startSN - t.startSN,
                                    o = e.fragmentHint ? e.fragments.concat(e.fragmentHint) : e.fragments,
                                    l = t.fragmentHint ? t.fragments.concat(t.fragmentHint) : t.fragments,
                                    u = n;
                                u <= a;
                                u++
                            ) {
                                var h = l[s + u],
                                    d = o[u];
                                i && !d && u < i && (d = e.fragments[u] = h), h && d && r(h, d);
                            }
                        })(t, e, function (t, i) {
                            var a;
                            t.relurl && (u = t.cc - i.cc),
                                Object(n.a)(t.startPTS) &&
                                    Object(n.a)(t.endPTS) &&
                                    ((i.start = i.startPTS = t.startPTS),
                                    (i.startDTS = t.startDTS),
                                    (i.appendedPTS = t.appendedPTS),
                                    (i.maxStartPTS = t.maxStartPTS),
                                    (i.endPTS = t.endPTS),
                                    (i.endDTS = t.endDTS),
                                    (i.minEndPTS = t.minEndPTS),
                                    (i.duration = t.endPTS - t.startPTS),
                                    i.duration && (l = i),
                                    (e.PTSKnown = e.alignedSliding = !0)),
                                (i.elementaryStreams = t.elementaryStreams),
                                (i.loader = t.loader),
                                (i.stats = t.stats),
                                (i.urlId = t.urlId),
                                t.initSegment ? ((i.initSegment = t.initSegment), (r = t.initSegment)) : (i.initSegment && i.initSegment.relurl != (null === (a = r) || void 0 === a ? void 0 : a.relurl)) || (i.initSegment = r);
                        }),
                        e.skippedSegments &&
                            ((e.deltaUpdateFailed = e.fragments.some(function (t) {
                                return !t;
                            })),
                            e.deltaUpdateFailed))
                    ) {
                        o.b.warn("[level-helper] Previous playlist missing segments skipped in delta playlist");
                        for (var h = e.skippedSegments; h--; ) e.fragments.shift();
                        (e.startSN = e.fragments[0].sn), (e.startCC = e.fragments[0].cc);
                    }
                    var d = e.fragments;
                    if (u) {
                        o.b.warn("discontinuity sliding from playlist, take drift into account");
                        for (var c = 0; c < d.length; c++) d[c].cc += u;
                    }
                    e.skippedSegments && (e.startCC = e.fragments[0].cc),
                        (function (t, e, r) {
                            if (t && e)
                                for (var i = 0, n = 0, a = t.length; n <= a; n++) {
                                    var s = t[n],
                                        o = e[n + i];
                                    s && o && s.index === o.index && s.fragment.sn === o.fragment.sn ? r(s, o) : i--;
                                }
                        })(t.partList, e.partList, function (t, e) {
                            (e.elementaryStreams = t.elementaryStreams), (e.stats = t.stats);
                        }),
                        l ? Y(e, l, l.startPTS, l.endPTS, l.startDTS, l.endDTS) : X(t, e),
                        d.length && (e.totalduration = e.edge - d[0].start),
                        (e.driftStartTime = t.driftStartTime),
                        (e.driftStart = t.driftStart);
                    var f = e.advancedDateTime;
                    if (e.advanced && f) {
                        var g = e.edge;
                        e.driftStart || ((e.driftStartTime = f), (e.driftStart = g)), (e.driftEndTime = f), (e.driftEnd = g);
                    } else (e.driftEndTime = t.driftEndTime), (e.driftEnd = t.driftEnd), (e.advancedDateTime = t.advancedDateTime);
                }
                function X(t, e) {
                    var r = e.startSN + e.skippedSegments - t.startSN,
                        i = t.fragments;
                    r < 0 || r >= i.length || z(e, i[r].start);
                }
                function z(t, e) {
                    if (e) {
                        for (var r = t.fragments, i = t.skippedSegments; i < r.length; i++) r[i].start += e;
                        t.fragmentHint && (t.fragmentHint.start += e);
                    }
                }
                var Q = (function () {
                    function t(t, e) {
                        (this.hls = void 0),
                            (this.timer = -1),
                            (this.canLoad = !1),
                            (this.retryCount = 0),
                            (this.log = void 0),
                            (this.warn = void 0),
                            (this.log = o.b.log.bind(o.b, e + ":")),
                            (this.warn = o.b.warn.bind(o.b, e + ":")),
                            (this.hls = t);
                    }
                    var e = t.prototype;
                    return (
                        (e.destroy = function () {
                            this.clearTimer(), (this.hls = this.log = this.warn = null);
                        }),
                        (e.onError = function (t, e) {
                            e.fatal && e.type === s.b.NETWORK_ERROR && this.clearTimer();
                        }),
                        (e.clearTimer = function () {
                            clearTimeout(this.timer), (this.timer = -1);
                        }),
                        (e.startLoad = function () {
                            (this.canLoad = !0), (this.retryCount = 0), this.loadPlaylist();
                        }),
                        (e.stopLoad = function () {
                            (this.canLoad = !1), this.clearTimer();
                        }),
                        (e.switchParams = function (t, e) {
                            var r = null == e ? void 0 : e.renditionReports;
                            if (r)
                                for (var i = 0; i < r.length; i++) {
                                    var a = r[i],
                                        s = "" + a.URI;
                                    if (s === t.substr(-s.length)) {
                                        var o = parseInt(a["LAST-MSN"]),
                                            l = parseInt(a["LAST-PART"]);
                                        if (e && this.hls.config.lowLatencyMode) {
                                            var u = Math.min(e.age - e.partTarget, e.targetduration);
                                            void 0 !== l && u > e.partTarget && (l += 1);
                                        }
                                        if (Object(n.a)(o)) return new K(o, Object(n.a)(l) ? l : void 0, U.No);
                                    }
                                }
                        }),
                        (e.loadPlaylist = function (t) {}),
                        (e.shouldLoadTrack = function (t) {
                            return this.canLoad && t && !!t.url && (!t.details || t.details.live);
                        }),
                        (e.playlistLoaded = function (t, e, r) {
                            var i = this,
                                n = e.details,
                                a = e.stats,
                                s = a.loading.end ? Math.max(0, self.performance.now() - a.loading.end) : 0;
                            if (((n.advancedDateTime = Date.now() - s), n.live || (null != r && r.live))) {
                                if (
                                    (n.reloaded(r), r && this.log("live playlist " + t + " " + (n.advanced ? "REFRESHED " + n.lastPartSn + "-" + n.lastPartIndex : "MISSED")), r && n.fragments.length > 0 && q(r, n), !this.canLoad || !n.live)
                                )
                                    return;
                                var o,
                                    l = void 0,
                                    u = void 0;
                                if (n.canBlockReload && n.endSN && n.advanced) {
                                    var h = this.hls.config.lowLatencyMode,
                                        d = n.lastPartSn,
                                        c = n.endSN,
                                        f = n.lastPartIndex,
                                        g = d === c;
                                    -1 !== f ? ((l = g ? c + 1 : d), (u = g ? (h ? 0 : f) : f + 1)) : (l = c + 1);
                                    var v = n.age,
                                        p = v + n.ageHeader,
                                        m = Math.min(p - n.partTarget, 1.5 * n.targetduration);
                                    if (m > 0) {
                                        if (r && m > r.tuneInGoal) this.warn("CDN Tune-in goal increased from: " + r.tuneInGoal + " to: " + m + " with playlist age: " + n.age), (m = 0);
                                        else {
                                            var y = Math.floor(m / n.targetduration);
                                            if (((l += y), void 0 !== u)) u += Math.round((m % n.targetduration) / n.partTarget);
                                            this.log("CDN Tune-in age: " + n.ageHeader + "s last advanced " + v.toFixed(2) + "s goal: " + m + " skip sn " + y + " to part " + u);
                                        }
                                        n.tuneInGoal = m;
                                    }
                                    if (((o = this.getDeliveryDirectives(n, e.deliveryDirectives, l, u)), h || !g)) return void this.loadPlaylist(o);
                                } else o = this.getDeliveryDirectives(n, e.deliveryDirectives, l, u);
                                var T = (function (t, e) {
                                    var r,
                                        i = 1e3 * t.levelTargetDuration,
                                        n = i / 2,
                                        a = t.age,
                                        s = a > 0 && a < 3 * i,
                                        o = e.loading.end - e.loading.start,
                                        l = t.availabilityDelay;
                                    if (!1 === t.updated)
                                        if (s) {
                                            var u = 333 * t.misses;
                                            (r = Math.max(Math.min(n, 2 * o), u)), (t.availabilityDelay = (t.availabilityDelay || 0) + r);
                                        } else r = n;
                                    else s ? ((l = Math.min(l || i / 2, a)), (t.availabilityDelay = l), (r = l + i - a)) : (r = i - o);
                                    return Math.round(r);
                                })(n, a);
                                void 0 !== l && n.canBlockReload && (T -= n.partTarget || 1),
                                    this.log("reload live playlist " + t + " in " + Math.round(T) + " ms"),
                                    (this.timer = self.setTimeout(function () {
                                        return i.loadPlaylist(o);
                                    }, T));
                            } else this.clearTimer();
                        }),
                        (e.getDeliveryDirectives = function (t, e, r, i) {
                            var n = (function (t, e) {
                                var r = t.canSkipUntil,
                                    i = t.canSkipDateRanges,
                                    n = t.endSN;
                                return r && (void 0 !== e ? e - n : 0) < r ? (i ? U.v2 : U.Yes) : U.No;
                            })(t, r);
                            return null != e && e.skip && t.deltaUpdateFailed && ((r = e.msn), (i = e.part), (n = U.No)), new K(r, i, n);
                        }),
                        (e.retryLoadingOrFail = function (t) {
                            var e,
                                r = this,
                                i = this.hls.config,
                                n = this.retryCount < i.levelLoadingMaxRetry;
                            if (n)
                                if ((this.retryCount++, t.details.indexOf("LoadTimeOut") > -1 && null !== (e = t.context) && void 0 !== e && e.deliveryDirectives))
                                    this.warn("retry playlist loading #" + this.retryCount + ' after "' + t.details + '"'), this.loadPlaylist();
                                else {
                                    var a = Math.min(Math.pow(2, this.retryCount) * i.levelLoadingRetryDelay, i.levelLoadingMaxRetryTimeout);
                                    (this.timer = self.setTimeout(function () {
                                        return r.loadPlaylist();
                                    }, a)),
                                        this.warn("retry playlist loading #" + this.retryCount + " in " + a + ' ms after "' + t.details + '"');
                                }
                            else this.warn('cannot recover from error "' + t.details + '"'), this.clearTimer(), (t.fatal = !0);
                            return n;
                        }),
                        t
                    );
                })();
                function $() {
                    return ($ =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                function J(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function Z(t, e) {
                    return (Z =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var tt,
                    et = /chrome|firefox/.test(navigator.userAgent.toLowerCase()),
                    rt = (function (t) {
                        var e, r;
                        function i(e) {
                            var r;
                            return (
                                ((r = t.call(this, e, "[level-controller]") || this)._levels = []),
                                (r._firstLevel = -1),
                                (r._startLevel = void 0),
                                (r.currentLevelIndex = -1),
                                (r.manualLevelIndex = -1),
                                (r.onParsedComplete = void 0),
                                r._registerListeners(),
                                r
                            );
                        }
                        (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), Z(e, r);
                        var n,
                            o,
                            l,
                            u = i.prototype;
                        return (
                            (u._registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MANIFEST_LOADED, this.onManifestLoaded, this),
                                    t.on(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.on(a.a.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this),
                                    t.on(a.a.FRAG_LOADED, this.onFragLoaded, this),
                                    t.on(a.a.ERROR, this.onError, this);
                            }),
                            (u._unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MANIFEST_LOADED, this.onManifestLoaded, this),
                                    t.off(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.off(a.a.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this),
                                    t.off(a.a.FRAG_LOADED, this.onFragLoaded, this),
                                    t.off(a.a.ERROR, this.onError, this);
                            }),
                            (u.destroy = function () {
                                this._unregisterListeners(), (this.manualLevelIndex = -1), (this._levels.length = 0), t.prototype.destroy.call(this);
                            }),
                            (u.startLoad = function () {
                                this._levels.forEach(function (t) {
                                    t.loadError = 0;
                                }),
                                    t.prototype.startLoad.call(this);
                            }),
                            (u.onManifestLoaded = function (t, e) {
                                var r,
                                    i,
                                    n = [],
                                    o = [],
                                    l = [],
                                    u = {},
                                    h = !1,
                                    d = !1,
                                    c = !1;
                                if (
                                    (e.levels.forEach(function (t) {
                                        var e = t.attrs;
                                        (h = h || !(!t.width || !t.height)), (d = d || !!t.videoCodec), (c = c || !!t.audioCodec), et && t.audioCodec && -1 !== t.audioCodec.indexOf("mp4a.40.34") && (t.audioCodec = void 0);
                                        var r = t.bitrate + "-" + t.attrs.RESOLUTION + "-" + t.attrs.CODECS;
                                        (i = u[r]) ? i.url.push(t.url) : ((i = new j(t)), (u[r] = i), n.push(i)), e && (e.AUDIO && H(i, "audio", e.AUDIO), e.SUBTITLES && H(i, "text", e.SUBTITLES));
                                    }),
                                    (h || d) &&
                                        c &&
                                        (n = n.filter(function (t) {
                                            var e = t.videoCodec,
                                                r = t.width,
                                                i = t.height;
                                            return !!e || !(!r || !i);
                                        })),
                                    (n = n.filter(function (t) {
                                        var e = t.audioCodec,
                                            r = t.videoCodec;
                                        return (!e || m(e, "audio")) && (!r || m(r, "video"));
                                    })),
                                    e.audioTracks &&
                                        V(
                                            (o = e.audioTracks.filter(function (t) {
                                                return !t.audioCodec || m(t.audioCodec, "audio");
                                            }))
                                        ),
                                    e.subtitles && V((l = e.subtitles)),
                                    n.length > 0)
                                ) {
                                    (r = n[0].bitrate),
                                        n.sort(function (t, e) {
                                            return t.bitrate - e.bitrate;
                                        }),
                                        (this._levels = n);
                                    for (var f = 0; f < n.length; f++)
                                        if (n[f].bitrate === r) {
                                            (this._firstLevel = f), this.log("manifest loaded, " + n.length + " level(s) found, first bitrate: " + r);
                                            break;
                                        }
                                    var g = c && !d,
                                        v = {
                                            levels: n,
                                            audioTracks: o,
                                            subtitleTracks: l,
                                            firstLevel: this._firstLevel,
                                            stats: e.stats,
                                            audio: c,
                                            video: d,
                                            altAudio:
                                                !g &&
                                                o.some(function (t) {
                                                    return !!t.url;
                                                }),
                                        };
                                    this.hls.trigger(a.a.MANIFEST_PARSED, v), (this.hls.config.autoStartLoad || this.hls.forceStartLoad) && this.hls.startLoad(this.hls.config.startPosition);
                                } else this.hls.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.MANIFEST_INCOMPATIBLE_CODECS_ERROR, fatal: !0, url: e.url, reason: "no level with compatible codecs found in manifest" });
                            }),
                            (u.onError = function (e, r) {
                                if ((t.prototype.onError.call(this, e, r), !r.fatal)) {
                                    var i = r.context,
                                        n = this._levels[this.currentLevelIndex];
                                    if (i && ((i.type === k.a.AUDIO_TRACK && n.audioGroupIds && i.groupId === n.audioGroupIds[n.urlId]) || (i.type === k.a.SUBTITLE_TRACK && n.textGroupIds && i.groupId === n.textGroupIds[n.urlId])))
                                        this.redundantFailover(this.currentLevelIndex);
                                    else {
                                        var a,
                                            o = !1,
                                            l = !0;
                                        switch (r.details) {
                                            case s.a.FRAG_LOAD_ERROR:
                                            case s.a.FRAG_LOAD_TIMEOUT:
                                            case s.a.KEY_LOAD_ERROR:
                                            case s.a.KEY_LOAD_TIMEOUT:
                                                if (r.frag) {
                                                    var u = this._levels[r.frag.level];
                                                    u ? (u.fragmentError++, u.fragmentError > this.hls.config.fragLoadingMaxRetry && (a = r.frag.level)) : (a = r.frag.level);
                                                }
                                                break;
                                            case s.a.LEVEL_LOAD_ERROR:
                                            case s.a.LEVEL_LOAD_TIMEOUT:
                                                i && (i.deliveryDirectives && (l = !1), (a = i.level)), (o = !0);
                                                break;
                                            case s.a.REMUX_ALLOC_ERROR:
                                                (a = r.level), (o = !0);
                                        }
                                        void 0 !== a && this.recoverLevel(r, a, o, l);
                                    }
                                }
                            }),
                            (u.recoverLevel = function (t, e, r, i) {
                                var n = t.details,
                                    a = this._levels[e];
                                if ((a.loadError++, r)) {
                                    if (!this.retryLoadingOrFail(t)) return void (this.currentLevelIndex = -1);
                                    t.levelRetry = !0;
                                }
                                if (i) {
                                    var s = a.url.length;
                                    if (s > 1 && a.loadError < s) (t.levelRetry = !0), this.redundantFailover(e);
                                    else if (-1 === this.manualLevelIndex) {
                                        var o = 0 === e ? this._levels.length - 1 : e - 1;
                                        this.currentLevelIndex !== o && 0 === this._levels[o].loadError && (this.warn(n + ": switch to " + o), (t.levelRetry = !0), (this.hls.nextAutoLevel = o));
                                    }
                                }
                            }),
                            (u.redundantFailover = function (t) {
                                var e = this._levels[t],
                                    r = e.url.length;
                                if (r > 1) {
                                    var i = (e.urlId + 1) % r;
                                    this.warn("Switching to redundant URL-id " + i),
                                        this._levels.forEach(function (t) {
                                            t.urlId = i;
                                        }),
                                        (this.level = t);
                                }
                            }),
                            (u.onFragLoaded = function (t, e) {
                                var r = e.frag;
                                if (void 0 !== r && r.type === k.b.MAIN) {
                                    var i = this._levels[r.level];
                                    void 0 !== i && ((i.fragmentError = 0), (i.loadError = 0));
                                }
                            }),
                            (u.onLevelLoaded = function (t, e) {
                                var r,
                                    i,
                                    n = e.level,
                                    a = e.details,
                                    s = this._levels[n];
                                if (!s) return this.warn("Invalid level index " + n), void (null !== (i = e.deliveryDirectives) && void 0 !== i && i.skip && (a.deltaUpdateFailed = !0));
                                n === this.currentLevelIndex
                                    ? (0 === s.fragmentError && ((s.loadError = 0), (this.retryCount = 0)), this.playlistLoaded(n, e, s.details))
                                    : null !== (r = e.deliveryDirectives) && void 0 !== r && r.skip && (a.deltaUpdateFailed = !0);
                            }),
                            (u.onAudioTrackSwitched = function (t, e) {
                                var r = this.hls.levels[this.currentLevelIndex];
                                if (r && r.audioGroupIds) {
                                    for (var i = -1, n = this.hls.audioTracks[e.id].groupId, a = 0; a < r.audioGroupIds.length; a++)
                                        if (r.audioGroupIds[a] === n) {
                                            i = a;
                                            break;
                                        }
                                    i !== r.urlId && ((r.urlId = i), this.startLoad());
                                }
                            }),
                            (u.loadPlaylist = function (t) {
                                var e = this.currentLevelIndex,
                                    r = this._levels[e];
                                if (this.canLoad && r && r.url.length > 0) {
                                    var i = r.urlId,
                                        n = r.url[i];
                                    if (t)
                                        try {
                                            n = t.addDirectives(n);
                                        } catch (t) {
                                            this.warn("Could not construct new URL with HLS Delivery Directives: " + t);
                                        }
                                    this.log("Attempt loading level index " + e + (t ? " at sn " + t.msn + " part " + t.part : "") + " with URL-id " + i + " " + n),
                                        this.clearTimer(),
                                        this.hls.trigger(a.a.LEVEL_LOADING, { url: n, level: e, id: i, deliveryDirectives: t || null });
                                }
                            }),
                            (u.removeLevel = function (t, e) {
                                var r = function (t, r) {
                                        return r !== e;
                                    },
                                    i = this._levels
                                        .filter(function (i, n) {
                                            return (
                                                n !== t ||
                                                (i.url.length > 1 &&
                                                    void 0 !== e &&
                                                    ((i.url = i.url.filter(r)), i.audioGroupIds && (i.audioGroupIds = i.audioGroupIds.filter(r)), i.textGroupIds && (i.textGroupIds = i.textGroupIds.filter(r)), (i.urlId = 0), !0))
                                            );
                                        })
                                        .map(function (t, e) {
                                            var r = t.details;
                                            return (
                                                null != r &&
                                                    r.fragments &&
                                                    r.fragments.forEach(function (t) {
                                                        t.level = e;
                                                    }),
                                                t
                                            );
                                        });
                                (this._levels = i), this.hls.trigger(a.a.LEVELS_UPDATED, { levels: i });
                            }),
                            (n = i),
                            (o = [
                                {
                                    key: "levels",
                                    get: function () {
                                        return 0 === this._levels.length ? null : this._levels;
                                    },
                                },
                                {
                                    key: "level",
                                    get: function () {
                                        return this.currentLevelIndex;
                                    },
                                    set: function (t) {
                                        var e,
                                            r = this._levels;
                                        if (0 !== r.length && (this.currentLevelIndex !== t || null === (e = r[t]) || void 0 === e || !e.details)) {
                                            if (t < 0 || t >= r.length) {
                                                var i = t < 0;
                                                if ((this.hls.trigger(a.a.ERROR, { type: s.b.OTHER_ERROR, details: s.a.LEVEL_SWITCH_ERROR, level: t, fatal: i, reason: "invalid level idx" }), i)) return;
                                                t = Math.min(t, r.length - 1);
                                            }
                                            this.clearTimer();
                                            var n = this.currentLevelIndex,
                                                o = r[n],
                                                l = r[t];
                                            this.log("switching to level " + t + " from " + n), (this.currentLevelIndex = t);
                                            var u = $({}, l, { level: t, maxBitrate: l.maxBitrate, uri: l.uri, urlId: l.urlId });
                                            delete u._urlId, this.hls.trigger(a.a.LEVEL_SWITCHING, u);
                                            var h = l.details;
                                            if (!h || h.live) {
                                                var d = this.switchParams(l.uri, null == o ? void 0 : o.details);
                                                this.loadPlaylist(d);
                                            }
                                        }
                                    },
                                },
                                {
                                    key: "manualLevel",
                                    get: function () {
                                        return this.manualLevelIndex;
                                    },
                                    set: function (t) {
                                        (this.manualLevelIndex = t), void 0 === this._startLevel && (this._startLevel = t), -1 !== t && (this.level = t);
                                    },
                                },
                                {
                                    key: "firstLevel",
                                    get: function () {
                                        return this._firstLevel;
                                    },
                                    set: function (t) {
                                        this._firstLevel = t;
                                    },
                                },
                                {
                                    key: "startLevel",
                                    get: function () {
                                        if (void 0 === this._startLevel) {
                                            var t = this.hls.config.startLevel;
                                            return void 0 !== t ? t : this._firstLevel;
                                        }
                                        return this._startLevel;
                                    },
                                    set: function (t) {
                                        this._startLevel = t;
                                    },
                                },
                                {
                                    key: "nextLoadLevel",
                                    get: function () {
                                        return -1 !== this.manualLevelIndex ? this.manualLevelIndex : this.hls.nextAutoLevel;
                                    },
                                    set: function (t) {
                                        (this.level = t), -1 === this.manualLevelIndex && (this.hls.nextAutoLevel = t);
                                    },
                                },
                            ]) && J(n.prototype, o),
                            l && J(n, l),
                            i
                        );
                    })(Q);
                !(function (t) {
                    (t.NOT_LOADED = "NOT_LOADED"), (t.BACKTRACKED = "BACKTRACKED"), (t.APPENDING = "APPENDING"), (t.PARTIAL = "PARTIAL"), (t.OK = "OK");
                })(tt || (tt = {}));
                var it = (function () {
                    function t(t) {
                        (this.activeFragment = null),
                            (this.activeParts = null),
                            (this.fragments = Object.create(null)),
                            (this.timeRanges = Object.create(null)),
                            (this.bufferPadding = 0.2),
                            (this.hls = void 0),
                            (this.hls = t),
                            this._registerListeners();
                    }
                    var e = t.prototype;
                    return (
                        (e._registerListeners = function () {
                            var t = this.hls;
                            t.on(a.a.BUFFER_APPENDED, this.onBufferAppended, this), t.on(a.a.FRAG_BUFFERED, this.onFragBuffered, this), t.on(a.a.FRAG_LOADED, this.onFragLoaded, this);
                        }),
                        (e._unregisterListeners = function () {
                            var t = this.hls;
                            t.off(a.a.BUFFER_APPENDED, this.onBufferAppended, this), t.off(a.a.FRAG_BUFFERED, this.onFragBuffered, this), t.off(a.a.FRAG_LOADED, this.onFragLoaded, this);
                        }),
                        (e.destroy = function () {
                            this._unregisterListeners(), (this.fragments = this.timeRanges = null);
                        }),
                        (e.getAppendedFrag = function (t, e) {
                            if (e === k.b.MAIN) {
                                var r = this.activeFragment,
                                    i = this.activeParts;
                                if (!r) return null;
                                if (i)
                                    for (var n = i.length; n--; ) {
                                        var a = i[n],
                                            s = a ? a.end : r.appendedPTS;
                                        if (a.start <= t && void 0 !== s && t <= s) return n > 9 && (this.activeParts = i.slice(n - 9)), a;
                                    }
                                else if (r.start <= t && void 0 !== r.appendedPTS && t <= r.appendedPTS) return r;
                            }
                            return this.getBufferedFrag(t, e);
                        }),
                        (e.getBufferedFrag = function (t, e) {
                            for (var r = this.fragments, i = Object.keys(r), n = i.length; n--; ) {
                                var a = r[i[n]];
                                if ((null == a ? void 0 : a.body.type) === e && a.buffered) {
                                    var s = a.body;
                                    if (s.start <= t && t <= s.end) return s;
                                }
                            }
                            return null;
                        }),
                        (e.detectEvictedFragments = function (t, e, r) {
                            var i = this;
                            Object.keys(this.fragments).forEach(function (n) {
                                var a = i.fragments[n];
                                if (a)
                                    if (a.buffered) {
                                        var s = a.range[t];
                                        s &&
                                            s.time.some(function (t) {
                                                var r = !i.isTimeBuffered(t.startPTS, t.endPTS, e);
                                                return r && i.removeFragment(a.body), r;
                                            });
                                    } else a.body.type === r && i.removeFragment(a.body);
                            });
                        }),
                        (e.detectPartialFragments = function (t) {
                            var e = this,
                                r = this.timeRanges,
                                i = t.frag,
                                n = t.part;
                            if (r && "initSegment" !== i.sn) {
                                var a = at(i),
                                    s = this.fragments[a];
                                s &&
                                    (Object.keys(r).forEach(function (t) {
                                        var a = i.elementaryStreams[t];
                                        if (a) {
                                            var o = r[t],
                                                l = null !== n || !0 === a.partial;
                                            s.range[t] = e.getBufferedTimes(i, n, l, o);
                                        }
                                    }),
                                    (s.backtrack = s.loaded = null),
                                    Object.keys(s.range).length ? (s.buffered = !0) : this.removeFragment(s.body));
                            }
                        }),
                        (e.fragBuffered = function (t) {
                            var e = at(t),
                                r = this.fragments[e];
                            r && ((r.backtrack = r.loaded = null), (r.buffered = !0));
                        }),
                        (e.getBufferedTimes = function (t, e, r, i) {
                            for (var n = { time: [], partial: r }, a = e ? e.start : t.start, s = e ? e.end : t.end, o = t.minEndPTS || s, l = t.maxStartPTS || a, u = 0; u < i.length; u++) {
                                var h = i.start(u) - this.bufferPadding,
                                    d = i.end(u) + this.bufferPadding;
                                if (l >= h && o <= d) {
                                    n.time.push({ startPTS: Math.max(a, i.start(u)), endPTS: Math.min(s, i.end(u)) });
                                    break;
                                }
                                if (a < d && s > h) (n.partial = !0), n.time.push({ startPTS: Math.max(a, i.start(u)), endPTS: Math.min(s, i.end(u)) });
                                else if (s <= h) break;
                            }
                            return n;
                        }),
                        (e.getPartialFragment = function (t) {
                            var e,
                                r,
                                i,
                                n = null,
                                a = 0,
                                s = this.bufferPadding,
                                o = this.fragments;
                            return (
                                Object.keys(o).forEach(function (l) {
                                    var u = o[l];
                                    u && nt(u) && ((r = u.body.start - s), (i = u.body.end + s), t >= r && t <= i && ((e = Math.min(t - r, i - t)), a <= e && ((n = u.body), (a = e))));
                                }),
                                n
                            );
                        }),
                        (e.getState = function (t) {
                            var e = at(t),
                                r = this.fragments[e];
                            return r ? (r.buffered ? (nt(r) ? tt.PARTIAL : tt.OK) : r.backtrack ? tt.BACKTRACKED : tt.APPENDING) : tt.NOT_LOADED;
                        }),
                        (e.backtrack = function (t, e) {
                            var r = at(t),
                                i = this.fragments[r];
                            if (!i || i.backtrack) return null;
                            var n = (i.backtrack = e || i.loaded);
                            return (i.loaded = null), n;
                        }),
                        (e.getBacktrackData = function (t) {
                            var e = at(t),
                                r = this.fragments[e];
                            if (r) {
                                var i,
                                    n = r.backtrack;
                                if (null != n && null !== (i = n.payload) && void 0 !== i && i.byteLength) return n;
                                this.removeFragment(t);
                            }
                            return null;
                        }),
                        (e.isTimeBuffered = function (t, e, r) {
                            for (var i, n, a = 0; a < r.length; a++) {
                                if (((i = r.start(a) - this.bufferPadding), (n = r.end(a) + this.bufferPadding), t >= i && e <= n)) return !0;
                                if (e <= i) return !1;
                            }
                            return !1;
                        }),
                        (e.onFragLoaded = function (t, e) {
                            var r = e.frag,
                                i = e.part;
                            if ("initSegment" !== r.sn && !r.bitrateTest && !i) {
                                var n = at(r);
                                this.fragments[n] = { body: r, loaded: e, backtrack: null, buffered: !1, range: Object.create(null) };
                            }
                        }),
                        (e.onBufferAppended = function (t, e) {
                            var r = this,
                                i = e.frag,
                                n = e.part,
                                a = e.timeRanges;
                            if (i.type === k.b.MAIN)
                                if (((this.activeFragment = i), n)) {
                                    var s = this.activeParts;
                                    s || (this.activeParts = s = []), s.push(n);
                                } else this.activeParts = null;
                            (this.timeRanges = a),
                                Object.keys(a).forEach(function (t) {
                                    var e = a[t];
                                    if ((r.detectEvictedFragments(t, e), !n)) for (var s = 0; s < e.length; s++) i.appendedPTS = Math.max(e.end(s), i.appendedPTS || 0);
                                });
                        }),
                        (e.onFragBuffered = function (t, e) {
                            this.detectPartialFragments(e);
                        }),
                        (e.hasFragment = function (t) {
                            var e = at(t);
                            return !!this.fragments[e];
                        }),
                        (e.removeFragmentsInRange = function (t, e, r) {
                            var i = this;
                            Object.keys(this.fragments).forEach(function (n) {
                                var a = i.fragments[n];
                                if (a && a.buffered) {
                                    var s = a.body;
                                    s.type === r && s.start < e && s.end > t && i.removeFragment(s);
                                }
                            });
                        }),
                        (e.removeFragment = function (t) {
                            var e = at(t);
                            (t.stats.loaded = 0), t.clearElementaryStreamInfo(), delete this.fragments[e];
                        }),
                        (e.removeAllFragments = function () {
                            (this.fragments = Object.create(null)), (this.activeFragment = null), (this.activeParts = null);
                        }),
                        t
                    );
                })();
                function nt(t) {
                    var e, r;
                    return t.buffered && ((null === (e = t.range.video) || void 0 === e ? void 0 : e.partial) || (null === (r = t.range.audio) || void 0 === r ? void 0 : r.partial));
                }
                function at(t) {
                    return t.type + "_" + t.level + "_" + t.urlId + "_" + t.sn;
                }
                var st = (function () {
                        function t() {
                            (this._boundTick = void 0), (this._tickTimer = null), (this._tickInterval = null), (this._tickCallCount = 0), (this._boundTick = this.tick.bind(this));
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                this.onHandlerDestroying(), this.onHandlerDestroyed();
                            }),
                            (e.onHandlerDestroying = function () {
                                this.clearNextTick(), this.clearInterval();
                            }),
                            (e.onHandlerDestroyed = function () {}),
                            (e.hasInterval = function () {
                                return !!this._tickInterval;
                            }),
                            (e.hasNextTick = function () {
                                return !!this._tickTimer;
                            }),
                            (e.setInterval = function (t) {
                                return !this._tickInterval && ((this._tickInterval = self.setInterval(this._boundTick, t)), !0);
                            }),
                            (e.clearInterval = function () {
                                return !!this._tickInterval && (self.clearInterval(this._tickInterval), (this._tickInterval = null), !0);
                            }),
                            (e.clearNextTick = function () {
                                return !!this._tickTimer && (self.clearTimeout(this._tickTimer), (this._tickTimer = null), !0);
                            }),
                            (e.tick = function () {
                                this._tickCallCount++, 1 === this._tickCallCount && (this.doTick(), this._tickCallCount > 1 && this.tickImmediate(), (this._tickCallCount = 0));
                            }),
                            (e.tickImmediate = function () {
                                this.clearNextTick(), (this._tickTimer = self.setTimeout(this._boundTick, 0));
                            }),
                            (e.doTick = function () {}),
                            t
                        );
                    })(),
                    ot = {
                        length: 0,
                        start: function () {
                            return 0;
                        },
                        end: function () {
                            return 0;
                        },
                    },
                    lt = (function () {
                        function t() {}
                        return (
                            (t.isBuffered = function (e, r) {
                                try {
                                    if (e) for (var i = t.getBuffered(e), n = 0; n < i.length; n++) if (r >= i.start(n) && r <= i.end(n)) return !0;
                                } catch (t) {}
                                return !1;
                            }),
                            (t.bufferInfo = function (e, r, i) {
                                try {
                                    if (e) {
                                        var n,
                                            a = t.getBuffered(e),
                                            s = [];
                                        for (n = 0; n < a.length; n++) s.push({ start: a.start(n), end: a.end(n) });
                                        return this.bufferedInfo(s, r, i);
                                    }
                                } catch (t) {}
                                return { len: 0, start: r, end: r, nextStart: void 0 };
                            }),
                            (t.bufferedInfo = function (t, e, r) {
                                (e = Math.max(0, e)),
                                    t.sort(function (t, e) {
                                        var r = t.start - e.start;
                                        return r || e.end - t.end;
                                    });
                                var i = [];
                                if (r)
                                    for (var n = 0; n < t.length; n++) {
                                        var a = i.length;
                                        if (a) {
                                            var s = i[a - 1].end;
                                            t[n].start - s < r ? t[n].end > s && (i[a - 1].end = t[n].end) : i.push(t[n]);
                                        } else i.push(t[n]);
                                    }
                                else i = t;
                                for (var o, l = 0, u = e, h = e, d = 0; d < i.length; d++) {
                                    var c = i[d].start,
                                        f = i[d].end;
                                    if (e + r >= c && e < f) (u = c), (l = (h = f) - e);
                                    else if (e + r < c) {
                                        o = c;
                                        break;
                                    }
                                }
                                return { len: l, start: u || 0, end: h || 0, nextStart: o };
                            }),
                            (t.getBuffered = function (t) {
                                try {
                                    return t.buffered;
                                } catch (t) {
                                    return o.b.log("failed to get media.buffered", t), ot;
                                }
                            }),
                            t
                        );
                    })(),
                    ut = function (t, e, r, i, n, a) {
                        void 0 === i && (i = 0),
                            void 0 === n && (n = -1),
                            void 0 === a && (a = !1),
                            (this.level = void 0),
                            (this.sn = void 0),
                            (this.part = void 0),
                            (this.id = void 0),
                            (this.size = void 0),
                            (this.partial = void 0),
                            (this.transmuxing = { start: 0, executeStart: 0, executeEnd: 0, end: 0 }),
                            (this.buffering = { audio: { start: 0, executeStart: 0, executeEnd: 0, end: 0 }, video: { start: 0, executeStart: 0, executeEnd: 0, end: 0 }, audiovideo: { start: 0, executeStart: 0, executeEnd: 0, end: 0 } }),
                            (this.level = t),
                            (this.sn = e),
                            (this.id = r),
                            (this.size = i),
                            (this.part = n),
                            (this.partial = a);
                    };
                function ht(t, e) {
                    if (t) {
                        var r = t.start + e;
                        (t.start = t.startPTS = r), (t.endPTS = r + t.duration);
                    }
                }
                function dt(t, e) {
                    for (var r = e.fragments, i = 0, n = r.length; i < n; i++) ht(r[i], t);
                    e.fragmentHint && ht(e.fragmentHint, t), (e.alignedSliding = !0);
                }
                function ct(t, e, r) {
                    e &&
                        (!(function (t, e, r) {
                            if (
                                (function (t, e, r) {
                                    return !(!e.details || !(r.endCC > r.startCC || (t && t.cc < r.startCC)));
                                })(t, r, e)
                            ) {
                                var i = (function (t, e) {
                                    var r = t.fragments,
                                        i = e.fragments;
                                    if (i.length && r.length) {
                                        var n = (function (t, e) {
                                            for (var r = null, i = 0, n = t.length; i < n; i++) {
                                                var a = t[i];
                                                if (a && a.cc === e) {
                                                    r = a;
                                                    break;
                                                }
                                            }
                                            return r;
                                        })(r, i[0].cc);
                                        if (n && (!n || n.startPTS)) return n;
                                        o.b.log("No frag in previous level to align on");
                                    } else o.b.log("No fragments to align");
                                })(r.details, e);
                                i && Object(n.a)(i.start) && (o.b.log("Adjusting PTS using last level due to CC increase within current level " + e.url), dt(i.start, e));
                            }
                        })(t, r, e),
                        !r.alignedSliding &&
                            e.details &&
                            (function (t, e) {
                                if (!e.fragments.length || !t.hasProgramDateTime || !e.hasProgramDateTime) return;
                                var r = e.fragments[0].programDateTime,
                                    i = t.fragments[0].programDateTime,
                                    a = (i - r) / 1e3 + e.fragments[0].start;
                                a && Object(n.a)(a) && (o.b.log("Adjusting PTS using programDateTime delta " + (i - r) + "ms, sliding:" + a.toFixed(3) + " " + t.url + " "), dt(a, t));
                            })(r, e.details),
                        r.alignedSliding || !e.details || r.skippedSegments || X(e.details, r));
                }
                function ft(t, e) {
                    var r = t.programDateTime;
                    if (r) {
                        var i = (r - e) / 1e3;
                        (t.start = t.startPTS = i), (t.endPTS = i + t.duration);
                    }
                }
                function gt(t, e) {
                    if (e.fragments.length && t.hasProgramDateTime && e.hasProgramDateTime) {
                        var r = e.fragments[0].programDateTime - 1e3 * e.fragments[0].start;
                        t.fragments.forEach(function (t) {
                            ft(t, r);
                        }),
                            t.fragmentHint && ft(t.fragmentHint, r),
                            (t.alignedSliding = !0);
                    }
                }
                var vt = {
                    search: function (t, e) {
                        for (var r = 0, i = t.length - 1, n = null, a = null; r <= i; ) {
                            var s = e((a = t[(n = ((r + i) / 2) | 0)]));
                            if (s > 0) r = n + 1;
                            else {
                                if (!(s < 0)) return a;
                                i = n - 1;
                            }
                        }
                        return null;
                    },
                };
                function pt(t, e, r) {
                    if (null === e || !Array.isArray(t) || !t.length || !Object(n.a)(e)) return null;
                    if (e < (t[0].programDateTime || 0)) return null;
                    if (e >= (t[t.length - 1].endProgramDateTime || 0)) return null;
                    r = r || 0;
                    for (var i = 0; i < t.length; ++i) {
                        var a = t[i];
                        if (Tt(e, r, a)) return a;
                    }
                    return null;
                }
                function mt(t, e, r, i) {
                    void 0 === r && (r = 0), void 0 === i && (i = 0);
                    var n = null;
                    if ((t ? (n = e[t.sn - e[0].sn + 1] || null) : 0 === r && 0 === e[0].start && (n = e[0]), n && 0 === yt(r, i, n))) return n;
                    var a = vt.search(e, yt.bind(null, r, i));
                    return a || n;
                }
                function yt(t, e, r) {
                    void 0 === t && (t = 0), void 0 === e && (e = 0);
                    var i = Math.min(e, r.duration + (r.deltaPTS ? r.deltaPTS : 0));
                    return r.start + r.duration - i <= t ? 1 : r.start - i > t && r.start ? -1 : 0;
                }
                function Tt(t, e, r) {
                    var i = 1e3 * Math.min(e, r.duration + (r.deltaPTS ? r.deltaPTS : 0));
                    return (r.endProgramDateTime || 0) - i > t;
                }
                function bt(t) {
                    var e = "function" == typeof Map ? new Map() : void 0;
                    return (bt = function (t) {
                        if (null === t || ((r = t), -1 === Function.toString.call(r).indexOf("[native code]"))) return t;
                        var r;
                        if ("function" != typeof t) throw new TypeError("Super expression must either be null or a function");
                        if (void 0 !== e) {
                            if (e.has(t)) return e.get(t);
                            e.set(t, i);
                        }
                        function i() {
                            return Et(t, arguments, At(this).constructor);
                        }
                        return (i.prototype = Object.create(t.prototype, { constructor: { value: i, enumerable: !1, writable: !0, configurable: !0 } })), Lt(i, t);
                    })(t);
                }
                function Et(t, e, r) {
                    return (Et = St()
                        ? Reflect.construct
                        : function (t, e, r) {
                              var i = [null];
                              i.push.apply(i, e);
                              var n = new (Function.bind.apply(t, i))();
                              return r && Lt(n, r.prototype), n;
                          }).apply(null, arguments);
                }
                function St() {
                    if ("undefined" == typeof Reflect || !Reflect.construct) return !1;
                    if (Reflect.construct.sham) return !1;
                    if ("function" == typeof Proxy) return !0;
                    try {
                        return Boolean.prototype.valueOf.call(Reflect.construct(Boolean, [], function () {})), !0;
                    } catch (t) {
                        return !1;
                    }
                }
                function Lt(t, e) {
                    return (Lt =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                function At(t) {
                    return (At = Object.setPrototypeOf
                        ? Object.getPrototypeOf
                        : function (t) {
                              return t.__proto__ || Object.getPrototypeOf(t);
                          })(t);
                }
                var Rt = Math.pow(2, 17),
                    Dt = (function () {
                        function t(t) {
                            (this.config = void 0), (this.loader = null), (this.partLoadTimeout = -1), (this.config = t);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                this.loader && (this.loader.destroy(), (this.loader = null));
                            }),
                            (e.abort = function () {
                                this.loader && this.loader.abort();
                            }),
                            (e.load = function (t, e) {
                                var r = this,
                                    i = t.url;
                                if (!i) return Promise.reject(new _t({ type: s.b.NETWORK_ERROR, details: s.a.FRAG_LOAD_ERROR, fatal: !1, frag: t, networkDetails: null }, "Fragment does not have a " + (i ? "part list" : "url")));
                                this.abort();
                                var n = this.config,
                                    a = n.fLoader,
                                    o = n.loader;
                                return new Promise(function (i, l) {
                                    r.loader && r.loader.destroy();
                                    var u = (r.loader = t.loader = a ? new a(n) : new o(n)),
                                        h = kt(t),
                                        d = { timeout: n.fragLoadingTimeOut, maxRetry: 0, retryDelay: 0, maxRetryDelay: n.fragLoadingMaxRetryTimeout, highWaterMark: Rt };
                                    (t.stats = u.stats),
                                        u.load(h, d, {
                                            onSuccess: function (e, n, a, s) {
                                                r.resetLoader(t, u), i({ frag: t, part: null, payload: e.data, networkDetails: s });
                                            },
                                            onError: function (e, i, n) {
                                                r.resetLoader(t, u), l(new _t({ type: s.b.NETWORK_ERROR, details: s.a.FRAG_LOAD_ERROR, fatal: !1, frag: t, response: e, networkDetails: n }));
                                            },
                                            onAbort: function (e, i, n) {
                                                r.resetLoader(t, u), l(new _t({ type: s.b.NETWORK_ERROR, details: s.a.INTERNAL_ABORTED, fatal: !1, frag: t, networkDetails: n }));
                                            },
                                            onTimeout: function (e, i, n) {
                                                r.resetLoader(t, u), l(new _t({ type: s.b.NETWORK_ERROR, details: s.a.FRAG_LOAD_TIMEOUT, fatal: !1, frag: t, networkDetails: n }));
                                            },
                                            onProgress: function (r, i, n, a) {
                                                e && e({ frag: t, part: null, payload: n, networkDetails: a });
                                            },
                                        });
                                });
                            }),
                            (e.loadPart = function (t, e, r) {
                                var i = this;
                                this.abort();
                                var n = this.config,
                                    a = n.fLoader,
                                    o = n.loader;
                                return new Promise(function (l, u) {
                                    i.loader && i.loader.destroy();
                                    var h = (i.loader = t.loader = a ? new a(n) : new o(n)),
                                        d = kt(t, e),
                                        c = { timeout: n.fragLoadingTimeOut, maxRetry: 0, retryDelay: 0, maxRetryDelay: n.fragLoadingMaxRetryTimeout, highWaterMark: Rt };
                                    (e.stats = h.stats),
                                        h.load(d, c, {
                                            onSuccess: function (n, a, s, o) {
                                                i.resetLoader(t, h), i.updateStatsFromPart(t, e);
                                                var u = { frag: t, part: e, payload: n.data, networkDetails: o };
                                                r(u), l(u);
                                            },
                                            onError: function (r, n, a) {
                                                i.resetLoader(t, h), u(new _t({ type: s.b.NETWORK_ERROR, details: s.a.FRAG_LOAD_ERROR, fatal: !1, frag: t, part: e, response: r, networkDetails: a }));
                                            },
                                            onAbort: function (r, n, a) {
                                                (t.stats.aborted = e.stats.aborted), i.resetLoader(t, h), u(new _t({ type: s.b.NETWORK_ERROR, details: s.a.INTERNAL_ABORTED, fatal: !1, frag: t, part: e, networkDetails: a }));
                                            },
                                            onTimeout: function (r, n, a) {
                                                i.resetLoader(t, h), u(new _t({ type: s.b.NETWORK_ERROR, details: s.a.FRAG_LOAD_TIMEOUT, fatal: !1, frag: t, part: e, networkDetails: a }));
                                            },
                                        });
                                });
                            }),
                            (e.updateStatsFromPart = function (t, e) {
                                var r = t.stats,
                                    i = e.stats,
                                    n = i.total;
                                if (((r.loaded += i.loaded), n)) {
                                    var a = Math.round(t.duration / e.duration),
                                        s = Math.min(Math.round(r.loaded / n), a),
                                        o = (a - s) * Math.round(r.loaded / s);
                                    r.total = r.loaded + o;
                                } else r.total = Math.max(r.loaded, r.total);
                                var l = r.loading,
                                    u = i.loading;
                                l.start ? (l.first += u.first - u.start) : ((l.start = u.start), (l.first = u.first)), (l.end = u.end);
                            }),
                            (e.resetLoader = function (t, e) {
                                (t.loader = null), this.loader === e && (self.clearTimeout(this.partLoadTimeout), (this.loader = null)), e.destroy();
                            }),
                            t
                        );
                    })();
                function kt(t, e) {
                    void 0 === e && (e = null);
                    var r = e || t,
                        i = { frag: t, part: e, responseType: "arraybuffer", url: r.url, headers: {}, rangeStart: 0, rangeEnd: 0 },
                        a = r.byteRangeStartOffset,
                        s = r.byteRangeEndOffset;
                    return Object(n.a)(a) && Object(n.a)(s) && ((i.rangeStart = a), (i.rangeEnd = s)), i;
                }
                var _t = (function (t) {
                        var e, r;
                        function i(e) {
                            for (var r, i = arguments.length, n = new Array(i > 1 ? i - 1 : 0), a = 1; a < i; a++) n[a - 1] = arguments[a];
                            return ((r = t.call.apply(t, [this].concat(n)) || this).data = void 0), (r.data = e), r;
                        }
                        return (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), Lt(e, r), i;
                    })(bt(Error)),
                    It = r(15),
                    Ct = {
                        toString: function (t) {
                            for (var e = "", r = t.length, i = 0; i < r; i++) e += "[" + t.start(i).toFixed(3) + "," + t.end(i).toFixed(3) + "]";
                            return e;
                        },
                    };
                function wt(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function Ot(t, e) {
                    return (Ot =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var xt = "STOPPED",
                    Pt = "IDLE",
                    Mt = "KEY_LOADING",
                    Ft = "FRAG_LOADING",
                    Nt = "FRAG_LOADING_WAITING_RETRY",
                    Ut = "WAITING_TRACK",
                    Bt = "PARSING",
                    Gt = "PARSED",
                    Kt = "BACKTRACKING",
                    jt = "ENDED",
                    Ht = "ERROR",
                    Vt = "WAITING_INIT_PTS",
                    Wt = "WAITING_LEVEL",
                    Yt = (function (t) {
                        var e, r;
                        function i(e, r, i) {
                            var n;
                            return (
                                ((n = t.call(this) || this).hls = void 0),
                                (n.fragPrevious = null),
                                (n.fragCurrent = null),
                                (n.fragmentTracker = void 0),
                                (n.transmuxer = null),
                                (n._state = xt),
                                (n.media = void 0),
                                (n.mediaBuffer = void 0),
                                (n.config = void 0),
                                (n.bitrateTest = !1),
                                (n.lastCurrentTime = 0),
                                (n.nextLoadPosition = 0),
                                (n.startPosition = 0),
                                (n.loadedmetadata = !1),
                                (n.fragLoadError = 0),
                                (n.retryDate = 0),
                                (n.levels = null),
                                (n.fragmentLoader = void 0),
                                (n.levelLastLoaded = null),
                                (n.startFragRequested = !1),
                                (n.decrypter = void 0),
                                (n.initPTS = []),
                                (n.onvseeking = null),
                                (n.onvended = null),
                                (n.logPrefix = ""),
                                (n.log = void 0),
                                (n.warn = void 0),
                                (n.logPrefix = i),
                                (n.log = o.b.log.bind(o.b, i + ":")),
                                (n.warn = o.b.warn.bind(o.b, i + ":")),
                                (n.hls = e),
                                (n.fragmentLoader = new Dt(e.config)),
                                (n.fragmentTracker = r),
                                (n.config = e.config),
                                (n.decrypter = new It.a(e, e.config)),
                                e.on(
                                    a.a.KEY_LOADED,
                                    n.onKeyLoaded,
                                    (function (t) {
                                        if (void 0 === t) throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
                                        return t;
                                    })(n)
                                ),
                                n
                            );
                        }
                        (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), Ot(e, r);
                        var u,
                            h,
                            d,
                            c = i.prototype;
                        return (
                            (c.doTick = function () {
                                this.onTickEnd();
                            }),
                            (c.onTickEnd = function () {}),
                            (c.startLoad = function (t) {}),
                            (c.stopLoad = function () {
                                this.fragmentLoader.abort();
                                var t = this.fragCurrent;
                                t && this.fragmentTracker.removeFragment(t), this.resetTransmuxer(), (this.fragCurrent = null), (this.fragPrevious = null), this.clearInterval(), this.clearNextTick(), (this.state = xt);
                            }),
                            (c._streamEnded = function (t, e) {
                                var r = this.fragCurrent,
                                    i = this.fragmentTracker;
                                if (!e.live && r && r.sn === e.endSN && !t.nextStart) {
                                    var n = i.getState(r);
                                    return n === tt.PARTIAL || n === tt.OK;
                                }
                                return !1;
                            }),
                            (c.onMediaAttached = function (t, e) {
                                var r = (this.media = this.mediaBuffer = e.media);
                                (this.onvseeking = this.onMediaSeeking.bind(this)), (this.onvended = this.onMediaEnded.bind(this)), r.addEventListener("seeking", this.onvseeking), r.addEventListener("ended", this.onvended);
                                var i = this.config;
                                this.levels && i.autoStartLoad && this.state === xt && this.startLoad(i.startPosition);
                            }),
                            (c.onMediaDetaching = function () {
                                var t = this.media;
                                null != t && t.ended && (this.log("MSE detaching and video ended, reset startPosition"), (this.startPosition = this.lastCurrentTime = 0)),
                                    t && (t.removeEventListener("seeking", this.onvseeking), t.removeEventListener("ended", this.onvended), (this.onvseeking = this.onvended = null)),
                                    (this.media = this.mediaBuffer = null),
                                    (this.loadedmetadata = !1),
                                    this.fragmentTracker.removeAllFragments(),
                                    this.stopLoad();
                            }),
                            (c.onMediaSeeking = function () {
                                var t = this.config,
                                    e = this.fragCurrent,
                                    r = this.media,
                                    i = this.mediaBuffer,
                                    a = this.state,
                                    s = r ? r.currentTime : 0,
                                    o = lt.bufferInfo(i || r, s, t.maxBufferHole);
                                if ((this.log("media seeking to " + (Object(n.a)(s) ? s.toFixed(3) : s) + ", state: " + a), a === jt)) this.resetLoadingState();
                                else if (e && !o.len) {
                                    var l = t.maxFragLookUpTolerance,
                                        u = e.start - l,
                                        h = s > e.start + e.duration + l;
                                    (s < u || h) && (h && e.loader && (this.log("seeking outside of buffer while fragment load in progress, cancel fragment load"), e.loader.abort()), this.resetLoadingState());
                                }
                                r && (this.lastCurrentTime = s), this.loadedmetadata || o.len || (this.nextLoadPosition = this.startPosition = s), this.tickImmediate();
                            }),
                            (c.onMediaEnded = function () {
                                this.startPosition = this.lastCurrentTime = 0;
                            }),
                            (c.onKeyLoaded = function (t, e) {
                                if (this.state === Mt && e.frag === this.fragCurrent && this.levels) {
                                    this.state = Pt;
                                    var r = this.levels[e.frag.level].details;
                                    r && this.loadFragment(e.frag, r, e.frag.start);
                                }
                            }),
                            (c.onHandlerDestroying = function () {
                                this.stopLoad(), t.prototype.onHandlerDestroying.call(this);
                            }),
                            (c.onHandlerDestroyed = function () {
                                (this.state = xt),
                                    this.hls.off(a.a.KEY_LOADED, this.onKeyLoaded, this),
                                    this.fragmentLoader && this.fragmentLoader.destroy(),
                                    this.decrypter && this.decrypter.destroy(),
                                    (this.hls = this.log = this.warn = this.decrypter = this.fragmentLoader = this.fragmentTracker = null),
                                    t.prototype.onHandlerDestroyed.call(this);
                            }),
                            (c.loadKey = function (t, e) {
                                this.log("Loading key for " + t.sn + " of [" + e.startSN + "-" + e.endSN + "], " + ("[stream-controller]" === this.logPrefix ? "level" : "track") + " " + t.level),
                                    (this.state = Mt),
                                    (this.fragCurrent = t),
                                    this.hls.trigger(a.a.KEY_LOADING, { frag: t });
                            }),
                            (c.loadFragment = function (t, e, r) {
                                this._loadFragForPlayback(t, e, r);
                            }),
                            (c._loadFragForPlayback = function (t, e, r) {
                                var i = this;
                                this._doFragLoad(t, e, r, function (e) {
                                    if (i.fragContextChanged(t)) return i.warn("Fragment " + t.sn + (e.part ? " p: " + e.part.index : "") + " of level " + t.level + " was dropped during download."), void i.fragmentTracker.removeFragment(t);
                                    t.stats.chunkCount++, i._handleFragmentLoadProgress(e);
                                })
                                    .then(function (e) {
                                        if (e) {
                                            i.fragLoadError = 0;
                                            var r = i.state;
                                            if (!i.fragContextChanged(t))
                                                return "payload" in e && (i.log("Loaded fragment " + t.sn + " of level " + t.level), i.hls.trigger(a.a.FRAG_LOADED, e), i.state === Kt)
                                                    ? (i.fragmentTracker.backtrack(t, e), void i.resetFragmentLoading(t))
                                                    : void i._handleFragmentLoadComplete(e);
                                            (r === Ft || r === Kt || (!i.fragCurrent && r === Bt)) && (i.fragmentTracker.removeFragment(t), (i.state = Pt));
                                        }
                                    })
                                    .catch(function (e) {
                                        i.warn(e), i.resetFragmentLoading(t);
                                    });
                            }),
                            (c.flushMainBuffer = function (t, e, r) {
                                if ((void 0 === r && (r = null), t - e)) {
                                    var i = { startOffset: t, endOffset: e, type: r };
                                    (this.fragLoadError = 0), this.hls.trigger(a.a.BUFFER_FLUSHING, i);
                                }
                            }),
                            (c._loadInitSegment = function (t) {
                                var e = this;
                                this._doFragLoad(t)
                                    .then(function (r) {
                                        if (!r || e.fragContextChanged(t) || !e.levels) throw new Error("init load aborted");
                                        return r;
                                    })
                                    .then(function (r) {
                                        var i = e.hls,
                                            n = r.payload,
                                            s = t.decryptdata;
                                        if (n && n.byteLength > 0 && s && s.key && s.iv && "AES-128" === s.method) {
                                            var o = self.performance.now();
                                            return e.decrypter.webCryptoDecrypt(new Uint8Array(n), s.key.buffer, s.iv.buffer).then(function (e) {
                                                var n = self.performance.now();
                                                return i.trigger(a.a.FRAG_DECRYPTED, { frag: t, payload: e, stats: { tstart: o, tdecrypt: n } }), (r.payload = e), r;
                                            });
                                        }
                                        return r;
                                    })
                                    .then(function (r) {
                                        var i = e.fragCurrent,
                                            n = e.hls,
                                            s = e.levels;
                                        if (!s) throw new Error("init load aborted, missing levels");
                                        s[t.level].details;
                                        var o = t.stats;
                                        (e.state = Pt),
                                            (e.fragLoadError = 0),
                                            (t.data = new Uint8Array(r.payload)),
                                            (o.parsing.start = o.buffering.start = self.performance.now()),
                                            (o.parsing.end = o.buffering.end = self.performance.now()),
                                            r.frag === i && n.trigger(a.a.FRAG_BUFFERED, { stats: o, frag: i, part: null, id: t.type }),
                                            e.tick();
                                    })
                                    .catch(function (r) {
                                        e.warn(r), e.resetFragmentLoading(t);
                                    });
                            }),
                            (c.fragContextChanged = function (t) {
                                var e = this.fragCurrent;
                                return !t || !e || t.level !== e.level || t.sn !== e.sn || t.urlId !== e.urlId;
                            }),
                            (c.fragBufferedComplete = function (t, e) {
                                var r = this.mediaBuffer ? this.mediaBuffer : this.media;
                                this.log(
                                    "Buffered " + t.type + " sn: " + t.sn + (e ? " part: " + e.index : "") + " of " + ("[stream-controller]" === this.logPrefix ? "level" : "track") + " " + t.level + " " + Ct.toString(lt.getBuffered(r))
                                ),
                                    (this.state = Pt),
                                    this.tick();
                            }),
                            (c._handleFragmentLoadComplete = function (t) {
                                var e = this.transmuxer;
                                if (e) {
                                    var r = t.frag,
                                        i = t.part,
                                        n = t.partsLoaded,
                                        a =
                                            !n ||
                                            0 === n.length ||
                                            n.some(function (t) {
                                                return !t;
                                            }),
                                        s = new ut(r.level, r.sn, r.stats.chunkCount + 1, 0, i ? i.index : -1, !a);
                                    e.flush(s);
                                }
                            }),
                            (c._handleFragmentLoadProgress = function (t) {}),
                            (c._doFragLoad = function (t, e, r, i) {
                                var s = this;
                                if ((void 0 === r && (r = null), !this.levels)) throw new Error("frag load aborted, missing levels");
                                if (((r = Math.max(t.start, r || 0)), this.config.lowLatencyMode && e)) {
                                    var o = e.partList;
                                    if (o && i) {
                                        r > t.end && e.fragmentHint && (t = e.fragmentHint);
                                        var l = this.getNextPart(o, t, r);
                                        if (l > -1) {
                                            var u = o[l];
                                            return (
                                                this.log(
                                                    "Loading part sn: " +
                                                        t.sn +
                                                        " p: " +
                                                        u.index +
                                                        " cc: " +
                                                        t.cc +
                                                        " of playlist [" +
                                                        e.startSN +
                                                        "-" +
                                                        e.endSN +
                                                        "] parts [0-" +
                                                        l +
                                                        "-" +
                                                        (o.length - 1) +
                                                        "] " +
                                                        ("[stream-controller]" === this.logPrefix ? "level" : "track") +
                                                        ": " +
                                                        t.level +
                                                        ", target: " +
                                                        parseFloat(r.toFixed(3))
                                                ),
                                                (this.nextLoadPosition = u.start + u.duration),
                                                (this.state = Ft),
                                                this.hls.trigger(a.a.FRAG_LOADING, { frag: t, part: o[l], targetBufferTime: r }),
                                                this.doFragPartsLoad(t, o, l, i).catch(function (t) {
                                                    return s.handleFragLoadError(t);
                                                })
                                            );
                                        }
                                        if (!t.url || this.loadedEndOfParts(o, r)) return Promise.resolve(null);
                                    }
                                }
                                return (
                                    this.log(
                                        "Loading fragment " +
                                            t.sn +
                                            " cc: " +
                                            t.cc +
                                            " " +
                                            (e ? "of [" + e.startSN + "-" + e.endSN + "] " : "") +
                                            ("[stream-controller]" === this.logPrefix ? "level" : "track") +
                                            ": " +
                                            t.level +
                                            ", target: " +
                                            parseFloat(r.toFixed(3))
                                    ),
                                    Object(n.a)(t.sn) && !this.bitrateTest && (this.nextLoadPosition = t.start + t.duration),
                                    (this.state = Ft),
                                    this.hls.trigger(a.a.FRAG_LOADING, { frag: t, targetBufferTime: r }),
                                    this.fragmentLoader.load(t, i).catch(function (t) {
                                        return s.handleFragLoadError(t);
                                    })
                                );
                            }),
                            (c.doFragPartsLoad = function (t, e, r, i) {
                                var n = this;
                                return new Promise(function (s, o) {
                                    var l = [];
                                    !(function r(u) {
                                        var h = e[u];
                                        n.fragmentLoader
                                            .loadPart(t, h, i)
                                            .then(function (i) {
                                                l[h.index] = i;
                                                var o = i.part;
                                                n.hls.trigger(a.a.FRAG_LOADED, i);
                                                var d = e[u + 1];
                                                if (!d || d.fragment !== t) return s({ frag: t, part: o, partsLoaded: l });
                                                r(u + 1);
                                            })
                                            .catch(o);
                                    })(r);
                                });
                            }),
                            (c.handleFragLoadError = function (t) {
                                var e = t.data;
                                return e && e.details === s.a.INTERNAL_ABORTED ? this.handleFragLoadAborted(e.frag, e.part) : this.hls.trigger(a.a.ERROR, e), null;
                            }),
                            (c._handleTransmuxerFlush = function (t) {
                                var e = this.getCurrentContext(t);
                                if (e && this.state === Bt) {
                                    var r = e.frag,
                                        i = e.part,
                                        n = e.level,
                                        a = self.performance.now();
                                    (r.stats.parsing.end = a), i && (i.stats.parsing.end = a), this.updateLevelTiming(r, i, n, t.partial);
                                } else this.fragCurrent || (this.state = Pt);
                            }),
                            (c.getCurrentContext = function (t) {
                                var e = this.levels,
                                    r = t.level,
                                    i = t.sn,
                                    n = t.part;
                                if (!e || !e[r]) return this.warn("Levels object was unset while buffering fragment " + i + " of level " + r + ". The current chunk will not be buffered."), null;
                                var a = e[r],
                                    s =
                                        n > -1
                                            ? (function (t, e, r) {
                                                  if (!t || !t.details) return null;
                                                  var i = t.details.partList;
                                                  if (i)
                                                      for (var n = i.length; n--; ) {
                                                          var a = i[n];
                                                          if (a.index === r && a.fragment.sn === e) return a;
                                                      }
                                                  return null;
                                              })(a, i, n)
                                            : null,
                                    o = s
                                        ? s.fragment
                                        : (function (t, e, r) {
                                              if (!t || !t.details) return null;
                                              var i = t.details,
                                                  n = i.fragments[e - i.startSN];
                                              return n || ((n = i.fragmentHint) && n.sn === e ? n : e < i.startSN && r && r.sn === e ? r : null);
                                          })(a, i, this.fragCurrent);
                                return o ? { frag: o, part: s, level: a } : null;
                            }),
                            (c.bufferFragmentData = function (t, e, r, i) {
                                if (t && this.state === Bt) {
                                    var n = t.data1,
                                        s = t.data2,
                                        o = n;
                                    if ((n && s && (o = Object(l.a)(n, s)), o && o.length)) {
                                        var u = { type: t.type, frag: e, part: r, chunkMeta: i, parent: e.type, data: o };
                                        this.hls.trigger(a.a.BUFFER_APPENDING, u), t.dropped && t.independent && !r && this.flushBufferGap(e);
                                    }
                                }
                            }),
                            (c.flushBufferGap = function (t) {
                                var e = this.media;
                                if (e)
                                    if (lt.isBuffered(e, e.currentTime)) {
                                        var r = e.currentTime,
                                            i = lt.bufferInfo(e, r, 0),
                                            n = t.duration,
                                            a = Math.min(2 * this.config.maxFragLookUpTolerance, 0.25 * n),
                                            s = Math.max(Math.min(t.start - a, i.end - a), r + a);
                                        t.start - s > a && this.flushMainBuffer(s, t.start);
                                    } else this.flushMainBuffer(0, t.start);
                            }),
                            (c.getFwdBufferInfo = function (t, e) {
                                var r = this.config,
                                    i = this.getLoadPosition();
                                if (!Object(n.a)(i)) return null;
                                var a = lt.bufferInfo(t, i, r.maxBufferHole);
                                if (0 === a.len && void 0 !== a.nextStart) {
                                    var s = this.fragmentTracker.getBufferedFrag(i, e);
                                    if (s && a.nextStart < s.end) return lt.bufferInfo(t, i, Math.max(a.nextStart, r.maxBufferHole));
                                }
                                return a;
                            }),
                            (c.getMaxBufferLength = function (t) {
                                var e,
                                    r = this.config;
                                return (e = t ? Math.max((8 * r.maxBufferSize) / t, r.maxBufferLength) : r.maxBufferLength), Math.min(e, r.maxMaxBufferLength);
                            }),
                            (c.reduceMaxBufferLength = function (t) {
                                var e = this.config,
                                    r = t || e.maxBufferLength;
                                return e.maxMaxBufferLength >= r && ((e.maxMaxBufferLength /= 2), this.warn("Reduce max buffer length to " + e.maxMaxBufferLength + "s"), !0);
                            }),
                            (c.getNextFragment = function (t, e) {
                                var r,
                                    i,
                                    n = e.fragments,
                                    a = n.length;
                                if (!a) return null;
                                var s,
                                    o = this.config,
                                    l = n[0].start;
                                if (e.live) {
                                    var u = o.initialLiveManifestSize;
                                    if (a < u) return this.warn("Not enough fragments to start playback (have: " + a + ", need: " + u + ")"), null;
                                    e.PTSKnown || this.startFragRequested || -1 !== this.startPosition || ((s = this.getInitialLiveFragment(e, n)), (this.startPosition = s ? this.hls.liveSyncPosition || s.start : t));
                                } else t <= l && (s = n[0]);
                                if (!s) {
                                    var h = o.lowLatencyMode ? e.partEnd : e.fragmentEnd;
                                    s = this.getFragmentAtPosition(t, h, e);
                                }
                                return null === (r = s) || void 0 === r || !r.initSegment || (null !== (i = s) && void 0 !== i && i.initSegment.data) || this.bitrateTest || (s = s.initSegment), s;
                            }),
                            (c.getNextPart = function (t, e, r) {
                                for (var i = -1, n = !1, a = !0, s = 0, o = t.length; s < o; s++) {
                                    var l = t[s];
                                    if (((a = a && !l.independent), i > -1 && r < l.start)) break;
                                    var u = l.loaded;
                                    !u && (n || l.independent || a) && l.fragment === e && (i = s), (n = u);
                                }
                                return i;
                            }),
                            (c.loadedEndOfParts = function (t, e) {
                                var r = t[t.length - 1];
                                return r && e > r.start && r.loaded;
                            }),
                            (c.getInitialLiveFragment = function (t, e) {
                                var r = this.fragPrevious,
                                    i = null;
                                if (r) {
                                    if ((t.hasProgramDateTime && (this.log("Live playlist, switching playlist, load frag with same PDT: " + r.programDateTime), (i = pt(e, r.endProgramDateTime, this.config.maxFragLookUpTolerance))), !i)) {
                                        var n = r.sn + 1;
                                        if (n >= t.startSN && n <= t.endSN) {
                                            var a = e[n - t.startSN];
                                            r.cc === a.cc && ((i = a), this.log("Live playlist, switching playlist, load frag with next SN: " + i.sn));
                                        }
                                        i ||
                                            ((i = (function (t, e) {
                                                return vt.search(t, function (t) {
                                                    return t.cc < e ? 1 : t.cc > e ? -1 : 0;
                                                });
                                            })(e, r.cc)) &&
                                                this.log("Live playlist, switching playlist, load frag with same CC: " + i.sn));
                                    }
                                } else {
                                    var s = this.hls.liveSyncPosition;
                                    null !== s && (i = this.getFragmentAtPosition(s, this.bitrateTest ? t.fragmentEnd : t.edge, t));
                                }
                                return i;
                            }),
                            (c.getFragmentAtPosition = function (t, e, r) {
                                var i,
                                    n = this.config,
                                    a = this.fragPrevious,
                                    s = r.fragments,
                                    o = r.endSN,
                                    l = r.fragmentHint,
                                    u = n.maxFragLookUpTolerance,
                                    h = !!(n.lowLatencyMode && r.partList && l);
                                (h && l && !this.bitrateTest && ((s = s.concat(l)), (o = l.sn)), t < e) ? (i = mt(a, s, t, t > e - u ? 0 : u)) : (i = s[s.length - 1]);
                                if (i) {
                                    var d = i.sn - r.startSN,
                                        c = a && i.level === a.level,
                                        f = s[d + 1];
                                    if (this.fragmentTracker.getState(i) === tt.BACKTRACKED) {
                                        i = null;
                                        for (var g = d; s[g] && this.fragmentTracker.getState(s[g]) === tt.BACKTRACKED; ) i = a ? s[g--] : s[--g];
                                        i || (i = f);
                                    } else a && i.sn === a.sn && !h && c && (i.sn < o && this.fragmentTracker.getState(f) !== tt.OK ? (this.log("SN " + i.sn + " just loaded, load next one: " + f.sn), (i = f)) : (i = null));
                                }
                                return i;
                            }),
                            (c.synchronizeToLiveEdge = function (t) {
                                var e = this.config,
                                    r = this.media;
                                if (r) {
                                    var i = this.hls.liveSyncPosition,
                                        n = r.currentTime,
                                        a = t.fragments[0].start,
                                        s = t.edge,
                                        o = n >= a - e.maxFragLookUpTolerance && n <= s;
                                    if (null !== i && r.duration > i && (n < i || !o)) {
                                        var l = void 0 !== e.liveMaxLatencyDuration ? e.liveMaxLatencyDuration : e.liveMaxLatencyDurationCount * t.targetduration;
                                        ((!o && r.readyState < 4) || n < s - l) &&
                                            (this.loadedmetadata || (this.nextLoadPosition = i),
                                            r.readyState && (this.warn("Playback: " + n.toFixed(3) + " is located too far from the end of live sliding playlist: " + s + ", reset currentTime to : " + i.toFixed(3)), (r.currentTime = i)));
                                    }
                                }
                            }),
                            (c.alignPlaylists = function (t, e) {
                                var r = this.levels,
                                    i = this.levelLastLoaded,
                                    a = this.fragPrevious,
                                    s = null !== i ? r[i] : null,
                                    o = t.fragments.length;
                                if (!o) return this.warn("No fragments in live playlist"), 0;
                                var l = t.fragments[0].start,
                                    u = !e,
                                    h = t.alignedSliding && Object(n.a)(l);
                                if (u || (!h && !l)) {
                                    ct(a, s, t);
                                    var d = t.fragments[0].start;
                                    return this.log("Live playlist sliding: " + d.toFixed(2) + " start-sn: " + (e ? e.startSN : "na") + "->" + t.startSN + " prev-sn: " + (a ? a.sn : "na") + " fragments: " + o), d;
                                }
                                return l;
                            }),
                            (c.waitForCdnTuneIn = function (t) {
                                return t.live && t.canBlockReload && t.tuneInGoal > Math.max(t.partHoldBack, 3 * t.partTarget);
                            }),
                            (c.setStartPosition = function (t, e) {
                                var r = this.startPosition;
                                if ((r < e && (r = -1), -1 === r || -1 === this.lastCurrentTime)) {
                                    var i = t.startTimeOffset;
                                    Object(n.a)(i)
                                        ? ((r = e + i),
                                          i < 0 && (r += t.totalduration),
                                          (r = Math.min(Math.max(e, r), e + t.totalduration)),
                                          this.log("Start time offset " + i + " found in playlist, adjust startPosition to " + r),
                                          (this.startPosition = r))
                                        : t.live
                                        ? (r = this.hls.liveSyncPosition || e)
                                        : (this.startPosition = r = 0),
                                        (this.lastCurrentTime = r);
                                }
                                this.nextLoadPosition = r;
                            }),
                            (c.getLoadPosition = function () {
                                var t = this.media,
                                    e = 0;
                                return this.loadedmetadata && t ? (e = t.currentTime) : this.nextLoadPosition && (e = this.nextLoadPosition), e;
                            }),
                            (c.handleFragLoadAborted = function (t, e) {
                                this.transmuxer && "initSegment" !== t.sn && t.stats.aborted && (this.warn("Fragment " + t.sn + (e ? " part" + e.index : "") + " of level " + t.level + " was aborted"), this.resetFragmentLoading(t));
                            }),
                            (c.resetFragmentLoading = function (t) {
                                (this.fragCurrent && this.fragContextChanged(t)) || (this.state = Pt);
                            }),
                            (c.onFragmentOrKeyLoadError = function (t, e) {
                                if (!e.fatal) {
                                    var r = e.frag;
                                    if (r && r.type === t) {
                                        this.fragCurrent;
                                        var i = this.config;
                                        if (this.fragLoadError + 1 <= i.fragLoadingMaxRetry) {
                                            if (this.resetLiveStartWhenNotLoaded(r.level)) return;
                                            var n = Math.min(Math.pow(2, this.fragLoadError) * i.fragLoadingRetryDelay, i.fragLoadingMaxRetryTimeout);
                                            this.warn("Fragment " + r.sn + " of " + t + " " + r.level + " failed to load, retrying in " + n + "ms"), (this.retryDate = self.performance.now() + n), this.fragLoadError++, (this.state = Nt);
                                        } else
                                            e.levelRetry
                                                ? (t === k.b.AUDIO && (this.fragCurrent = null), (this.fragLoadError = 0), (this.state = Pt))
                                                : (o.b.error(e.details + " reaches max retry, redispatch as fatal ..."), (e.fatal = !0), this.hls.stopLoad(), (this.state = Ht));
                                    }
                                }
                            }),
                            (c.afterBufferFlushed = function (t, e, r) {
                                if (t) {
                                    var i = lt.getBuffered(t);
                                    this.fragmentTracker.detectEvictedFragments(e, i, r), this.state === jt && this.resetLoadingState();
                                }
                            }),
                            (c.resetLoadingState = function () {
                                (this.fragCurrent = null), (this.fragPrevious = null), (this.state = Pt);
                            }),
                            (c.resetLiveStartWhenNotLoaded = function (t) {
                                if (!this.loadedmetadata) {
                                    this.startFragRequested = !1;
                                    var e = this.levels ? this.levels[t].details : null;
                                    if (null != e && e.live) return (this.startPosition = -1), this.setStartPosition(e, 0), this.resetLoadingState(), !0;
                                    this.nextLoadPosition = this.startPosition;
                                }
                                return !1;
                            }),
                            (c.updateLevelTiming = function (t, e, r, i) {
                                var n = this,
                                    s = r.details;
                                Object.keys(t.elementaryStreams).reduce(function (e, o) {
                                    var l = t.elementaryStreams[o];
                                    if (l) {
                                        var u = l.endPTS - l.startPTS;
                                        if (u <= 0) return n.warn("Could not parse fragment " + t.sn + " " + o + " duration reliably (" + u + ") resetting transmuxer to fallback to playlist timing"), n.resetTransmuxer(), e || !1;
                                        var h = i ? 0 : Y(s, t, l.startPTS, l.endPTS, l.startDTS, l.endDTS);
                                        return n.hls.trigger(a.a.LEVEL_PTS_UPDATED, { details: s, level: r, drift: h, type: o, frag: t, start: l.startPTS, end: l.endPTS }), !0;
                                    }
                                    return e;
                                }, !1)
                                    ? ((this.state = Gt), this.hls.trigger(a.a.FRAG_PARSED, { frag: t, part: e }))
                                    : this.resetLoadingState();
                            }),
                            (c.resetTransmuxer = function () {
                                this.transmuxer && (this.transmuxer.destroy(), (this.transmuxer = null));
                            }),
                            (u = i),
                            (h = [
                                {
                                    key: "state",
                                    get: function () {
                                        return this._state;
                                    },
                                    set: function (t) {
                                        var e = this._state;
                                        e !== t && ((this._state = t), this.log(e + "->" + t));
                                    },
                                },
                            ]) && wt(u.prototype, h),
                            d && wt(u, d),
                            i
                        );
                    })(st);
                function qt() {
                    return self.MediaSource || self.WebKitMediaSource;
                }
                function Xt() {
                    return self.SourceBuffer || self.WebKitSourceBuffer;
                }
                var zt = r(18),
                    Qt = r(10),
                    $t = r(14),
                    Jt = qt() || {
                        isTypeSupported: function () {
                            return !1;
                        },
                    },
                    Zt = (function () {
                        function t(t, e, r, i) {
                            var n = this;
                            (this.hls = void 0),
                                (this.id = void 0),
                                (this.observer = void 0),
                                (this.frag = null),
                                (this.part = null),
                                (this.worker = void 0),
                                (this.onwmsg = void 0),
                                (this.transmuxer = null),
                                (this.onTransmuxComplete = void 0),
                                (this.onFlush = void 0),
                                (this.hls = t),
                                (this.id = e),
                                (this.onTransmuxComplete = r),
                                (this.onFlush = i);
                            var l = t.config,
                                u = function (e, r) {
                                    ((r = r || {}).frag = n.frag), (r.id = n.id), t.trigger(e, r);
                                };
                            (this.observer = new $t.EventEmitter()), this.observer.on(a.a.FRAG_DECRYPTED, u), this.observer.on(a.a.ERROR, u);
                            var h = { mp4: Jt.isTypeSupported("video/mp4"), mpeg: Jt.isTypeSupported("audio/mpeg"), mp3: Jt.isTypeSupported('audio/mp4; codecs="mp3"') },
                                d = navigator.vendor;
                            if (l.enableWorker && "undefined" != typeof Worker) {
                                var c;
                                o.b.log("demuxing in webworker");
                                try {
                                    (c = this.worker = zt(19)),
                                        (this.onwmsg = this.onWorkerMessage.bind(this)),
                                        c.addEventListener("message", this.onwmsg),
                                        (c.onerror = function (e) {
                                            t.trigger(a.a.ERROR, { type: s.b.OTHER_ERROR, details: s.a.INTERNAL_EXCEPTION, fatal: !0, event: "demuxerWorker", error: new Error(e.message + "  (" + e.filename + ":" + e.lineno + ")") });
                                        }),
                                        c.postMessage({ cmd: "init", typeSupported: h, vendor: d, id: e, config: JSON.stringify(l) });
                                } catch (t) {
                                    o.b.warn("Error in worker:", t),
                                        o.b.error("Error while initializing DemuxerWorker, fallback to inline"),
                                        c && self.URL.revokeObjectURL(c.objectURL),
                                        (this.transmuxer = new Qt.c(this.observer, h, l, d, e)),
                                        (this.worker = null);
                                }
                            } else this.transmuxer = new Qt.c(this.observer, h, l, d, e);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                var t = this.worker;
                                if (t) t.removeEventListener("message", this.onwmsg), t.terminate(), (this.worker = null);
                                else {
                                    var e = this.transmuxer;
                                    e && (e.destroy(), (this.transmuxer = null));
                                }
                                var r = this.observer;
                                r && r.removeAllListeners(), (this.observer = null);
                            }),
                            (e.push = function (t, e, r, i, n, a, s, l, u, h) {
                                var d = this;
                                u.transmuxing.start = self.performance.now();
                                var c = this.transmuxer,
                                    f = this.worker,
                                    g = a ? a.start : n.start,
                                    v = n.decryptdata,
                                    p = this.frag,
                                    m = !(p && n.cc === p.cc),
                                    y = !(p && u.level === p.level),
                                    T = p ? u.sn - p.sn : -1,
                                    b = this.part ? u.part - this.part.index : 1,
                                    E = !y && (1 === T || (0 === T && 1 === b)),
                                    S = self.performance.now();
                                (y || T || 0 === n.stats.parsing.start) && (n.stats.parsing.start = S), !a || (!b && E) || (a.stats.parsing.start = S);
                                var L = new Qt.b(m, E, l, y, g);
                                if (!E || m) {
                                    o.b.log(
                                        "[transmuxer-interface, " +
                                            n.type +
                                            "]: Starting new transmux session for sn: " +
                                            u.sn +
                                            " p: " +
                                            u.part +
                                            " level: " +
                                            u.level +
                                            " id: " +
                                            u.id +
                                            "\n        discontinuity: " +
                                            m +
                                            "\n        trackSwitch: " +
                                            y +
                                            "\n        contiguous: " +
                                            E +
                                            "\n        accurateTimeOffset: " +
                                            l +
                                            "\n        timeOffset: " +
                                            g
                                    );
                                    var A = new Qt.a(r, i, e, s, h);
                                    this.configureTransmuxer(A);
                                }
                                if (((this.frag = n), (this.part = a), f)) f.postMessage({ cmd: "demux", data: t, decryptdata: v, chunkMeta: u, state: L }, t instanceof ArrayBuffer ? [t] : []);
                                else if (c) {
                                    var R = c.push(t, v, u, L);
                                    Object(Qt.d)(R)
                                        ? R.then(function (t) {
                                              d.handleTransmuxComplete(t);
                                          })
                                        : this.handleTransmuxComplete(R);
                                }
                            }),
                            (e.flush = function (t) {
                                var e = this;
                                t.transmuxing.start = self.performance.now();
                                var r = this.transmuxer,
                                    i = this.worker;
                                if (i) i.postMessage({ cmd: "flush", chunkMeta: t });
                                else if (r) {
                                    var n = r.flush(t);
                                    Object(Qt.d)(n)
                                        ? n.then(function (r) {
                                              e.handleFlushResult(r, t);
                                          })
                                        : this.handleFlushResult(n, t);
                                }
                            }),
                            (e.handleFlushResult = function (t, e) {
                                var r = this;
                                t.forEach(function (t) {
                                    r.handleTransmuxComplete(t);
                                }),
                                    this.onFlush(e);
                            }),
                            (e.onWorkerMessage = function (t) {
                                var e = t.data,
                                    r = this.hls;
                                switch (e.event) {
                                    case "init":
                                        self.URL.revokeObjectURL(this.worker.objectURL);
                                        break;
                                    case "transmuxComplete":
                                        this.handleTransmuxComplete(e.data);
                                        break;
                                    case "flush":
                                        this.onFlush(e.data);
                                        break;
                                    default:
                                        (e.data = e.data || {}), (e.data.frag = this.frag), (e.data.id = this.id), r.trigger(e.event, e.data);
                                }
                            }),
                            (e.configureTransmuxer = function (t) {
                                var e = this.worker,
                                    r = this.transmuxer;
                                e ? e.postMessage({ cmd: "configure", config: t }) : r && r.configure(t);
                            }),
                            (e.handleTransmuxComplete = function (t) {
                                (t.chunkMeta.transmuxing.end = self.performance.now()), this.onTransmuxComplete(t);
                            }),
                            t
                        );
                    })(),
                    te = (function () {
                        function t(t, e, r, i) {
                            (this.config = void 0),
                                (this.media = void 0),
                                (this.fragmentTracker = void 0),
                                (this.hls = void 0),
                                (this.nudgeRetry = 0),
                                (this.stallReported = !1),
                                (this.stalled = null),
                                (this.moved = !1),
                                (this.seeking = !1),
                                (this.config = t),
                                (this.media = e),
                                (this.fragmentTracker = r),
                                (this.hls = i);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                this.hls = this.fragmentTracker = this.media = null;
                            }),
                            (e.poll = function (t) {
                                var e = this.config,
                                    r = this.media,
                                    i = this.stalled,
                                    n = r.currentTime,
                                    a = r.seeking,
                                    s = this.seeking && !a,
                                    l = !this.seeking && a;
                                if (((this.seeking = a), n === t)) {
                                    if (((l || s) && (this.stalled = null), !r.paused && !r.ended && 0 !== r.playbackRate && lt.getBuffered(r).length)) {
                                        var u = lt.bufferInfo(r, n, 0),
                                            h = u.len > 0,
                                            d = u.nextStart || 0;
                                        if (h || d) {
                                            if (a) {
                                                var c = u.len > 2,
                                                    f = !d || (d - n > 2 && !this.fragmentTracker.getPartialFragment(n));
                                                if (c || f) return;
                                                this.moved = !1;
                                            }
                                            if (!this.moved && null !== this.stalled) {
                                                var g,
                                                    v = Math.max(d, u.start || 0) - n,
                                                    p = this.hls.levels ? this.hls.levels[this.hls.currentLevel] : null,
                                                    m = (null == p || null === (g = p.details) || void 0 === g ? void 0 : g.live) ? 2 * p.details.targetduration : 2;
                                                if (v > 0 && v <= m) return void this._trySkipBufferHole(null);
                                            }
                                            var y = self.performance.now();
                                            if (null !== i) {
                                                var T = y - i;
                                                !a && T >= 250 && this._reportStall(u.len);
                                                var b = lt.bufferInfo(r, n, e.maxBufferHole);
                                                this._tryFixBufferStall(b, T);
                                            } else this.stalled = y;
                                        }
                                    }
                                } else if (((this.moved = !0), null !== i)) {
                                    if (this.stallReported) {
                                        var E = self.performance.now() - i;
                                        o.b.warn("playback not stuck anymore @" + n + ", after " + Math.round(E) + "ms"), (this.stallReported = !1);
                                    }
                                    (this.stalled = null), (this.nudgeRetry = 0);
                                }
                            }),
                            (e._tryFixBufferStall = function (t, e) {
                                var r = this.config,
                                    i = this.fragmentTracker,
                                    n = this.media.currentTime,
                                    a = i.getPartialFragment(n);
                                if (a && this._trySkipBufferHole(a)) return;
                                t.len > r.maxBufferHole && e > 1e3 * r.highBufferWatchdogPeriod && (o.b.warn("Trying to nudge playhead over buffer-hole"), (this.stalled = null), this._tryNudgeBuffer());
                            }),
                            (e._reportStall = function (t) {
                                var e = this.hls,
                                    r = this.media;
                                this.stallReported ||
                                    ((this.stallReported = !0),
                                    o.b.warn("Playback stalling at @" + r.currentTime + " due to low buffer (buffer=" + t + ")"),
                                    e.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_STALLED_ERROR, fatal: !1, buffer: t }));
                            }),
                            (e._trySkipBufferHole = function (t) {
                                for (var e = this.config, r = this.hls, i = this.media, n = i.currentTime, l = 0, u = lt.getBuffered(i), h = 0; h < u.length; h++) {
                                    var d = u.start(h);
                                    if (n + e.maxBufferHole >= l && n < d) {
                                        var c = Math.max(d + 0.05, i.currentTime + 0.1);
                                        return (
                                            o.b.warn("skipping hole, adjusting currentTime from " + n + " to " + c),
                                            (this.moved = !0),
                                            (this.stalled = null),
                                            (i.currentTime = c),
                                            t && r.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_SEEK_OVER_HOLE, fatal: !1, reason: "fragment loaded with buffer holes, seeking from " + n + " to " + c, frag: t }),
                                            c
                                        );
                                    }
                                    l = u.end(h);
                                }
                                return 0;
                            }),
                            (e._tryNudgeBuffer = function () {
                                var t = this.config,
                                    e = this.hls,
                                    r = this.media,
                                    i = r.currentTime,
                                    n = (this.nudgeRetry || 0) + 1;
                                if (((this.nudgeRetry = n), n < t.nudgeMaxRetry)) {
                                    var l = i + n * t.nudgeOffset;
                                    o.b.warn("Nudging 'currentTime' from " + i + " to " + l), (r.currentTime = l), e.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_NUDGE_ON_STALL, fatal: !1 });
                                } else
                                    o.b.error("Playhead still not moving while enough data buffered @" + i + " after " + t.nudgeMaxRetry + " nudges"),
                                        e.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_STALLED_ERROR, fatal: !0 });
                            }),
                            t
                        );
                    })();
                function ee(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function re(t, e) {
                    return (re =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var ie = (function (t) {
                        var e, r;
                        function i(e, r) {
                            var i;
                            return (
                                ((i = t.call(this, e, r, "[stream-controller]") || this).audioCodecSwap = !1),
                                (i.gapController = null),
                                (i.level = -1),
                                (i._forceStartLoad = !1),
                                (i.altAudio = !1),
                                (i.audioOnly = !1),
                                (i.fragPlaying = null),
                                (i.onvplaying = null),
                                (i.onvseeked = null),
                                (i.fragLastKbps = 0),
                                (i.stalled = !1),
                                (i.couldBacktrack = !1),
                                (i.audioCodecSwitch = !1),
                                (i.videoBuffer = null),
                                i._registerListeners(),
                                i
                            );
                        }
                        (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), re(e, r);
                        var l,
                            h,
                            d,
                            c = i.prototype;
                        return (
                            (c._registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.on(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                    t.on(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.on(a.a.FRAG_LOAD_EMERGENCY_ABORTED, this.onFragLoadEmergencyAborted, this),
                                    t.on(a.a.ERROR, this.onError, this),
                                    t.on(a.a.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this),
                                    t.on(a.a.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this),
                                    t.on(a.a.BUFFER_CREATED, this.onBufferCreated, this),
                                    t.on(a.a.BUFFER_FLUSHED, this.onBufferFlushed, this),
                                    t.on(a.a.LEVELS_UPDATED, this.onLevelsUpdated, this),
                                    t.on(a.a.FRAG_BUFFERED, this.onFragBuffered, this);
                            }),
                            (c._unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.off(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.off(a.a.FRAG_LOAD_EMERGENCY_ABORTED, this.onFragLoadEmergencyAborted, this),
                                    t.off(a.a.ERROR, this.onError, this),
                                    t.off(a.a.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this),
                                    t.off(a.a.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this),
                                    t.off(a.a.BUFFER_CREATED, this.onBufferCreated, this),
                                    t.off(a.a.BUFFER_FLUSHED, this.onBufferFlushed, this),
                                    t.off(a.a.LEVELS_UPDATED, this.onLevelsUpdated, this),
                                    t.off(a.a.FRAG_BUFFERED, this.onFragBuffered, this);
                            }),
                            (c.onHandlerDestroying = function () {
                                this._unregisterListeners(), this.onMediaDetaching();
                            }),
                            (c.startLoad = function (t) {
                                if (this.levels) {
                                    var e = this.lastCurrentTime,
                                        r = this.hls;
                                    if ((this.stopLoad(), this.setInterval(100), (this.level = -1), (this.fragLoadError = 0), !this.startFragRequested)) {
                                        var i = r.startLevel;
                                        -1 === i && (r.config.testBandwidth ? ((i = 0), (this.bitrateTest = !0)) : (i = r.nextAutoLevel)), (this.level = r.nextLoadLevel = i), (this.loadedmetadata = !1);
                                    }
                                    e > 0 && -1 === t && (this.log("Override startPosition with lastCurrentTime @" + e.toFixed(3)), (t = e)),
                                        (this.state = Pt),
                                        (this.nextLoadPosition = this.startPosition = this.lastCurrentTime = t),
                                        this.tick();
                                } else (this._forceStartLoad = !0), (this.state = xt);
                            }),
                            (c.stopLoad = function () {
                                (this._forceStartLoad = !1), t.prototype.stopLoad.call(this);
                            }),
                            (c.doTick = function () {
                                switch (this.state) {
                                    case Pt:
                                        this.doTickIdle();
                                        break;
                                    case Wt:
                                        var t,
                                            e = this.levels,
                                            r = this.level,
                                            i = null == e || null === (t = e[r]) || void 0 === t ? void 0 : t.details;
                                        if (i && (!i.live || this.levelLastLoaded === this.level)) {
                                            if (this.waitForCdnTuneIn(i)) break;
                                            this.state = Pt;
                                            break;
                                        }
                                        break;
                                    case Nt:
                                        var n,
                                            a = self.performance.now(),
                                            s = this.retryDate;
                                        (!s || a >= s || (null !== (n = this.media) && void 0 !== n && n.seeking)) && (this.log("retryDate reached, switch back to IDLE state"), (this.state = Pt));
                                }
                                this.onTickEnd();
                            }),
                            (c.onTickEnd = function () {
                                t.prototype.onTickEnd.call(this), this.checkBuffer(), this.checkFragmentChanged();
                            }),
                            (c.doTickIdle = function () {
                                var t,
                                    e,
                                    r = this.hls,
                                    i = this.levelLastLoaded,
                                    n = this.levels,
                                    s = this.media,
                                    o = r.config,
                                    l = r.nextLoadLevel;
                                if (null !== i && (s || (!this.startFragRequested && o.startFragPrefetch)) && (!this.altAudio || !this.audioOnly) && n && n[l]) {
                                    var h = n[l];
                                    this.level = r.nextLoadLevel = l;
                                    var d = h.details;
                                    if (!d || this.state === Wt || (d.live && this.levelLastLoaded !== l)) this.state = Wt;
                                    else {
                                        var c = this.getFwdBufferInfo(this.mediaBuffer ? this.mediaBuffer : s, k.b.MAIN);
                                        if (null !== c)
                                            if (!(c.len >= this.getMaxBufferLength(h.maxBitrate))) {
                                                if (this._streamEnded(c, d)) {
                                                    var f = {};
                                                    return this.altAudio && (f.type = "video"), this.hls.trigger(a.a.BUFFER_EOS, f), void (this.state = jt);
                                                }
                                                var g = c.end,
                                                    v = this.getNextFragment(g, d);
                                                if (this.couldBacktrack && !this.fragPrevious && v && "initSegment" !== v.sn) {
                                                    var p = v.sn - d.startSN;
                                                    p > 1 && ((v = d.fragments[p - 1]), this.fragmentTracker.removeFragment(v));
                                                }
                                                if (v && this.fragmentTracker.getState(v) === tt.OK && this.nextLoadPosition > g) {
                                                    var m = this.audioOnly && !this.altAudio ? u.a.AUDIO : u.a.VIDEO;
                                                    this.afterBufferFlushed(s, m, k.b.MAIN), (v = this.getNextFragment(this.nextLoadPosition, d));
                                                }
                                                v &&
                                                    (!v.initSegment || v.initSegment.data || this.bitrateTest || (v = v.initSegment),
                                                    "identity" !== (null === (t = v.decryptdata) || void 0 === t ? void 0 : t.keyFormat) || (null !== (e = v.decryptdata) && void 0 !== e && e.key)
                                                        ? this.loadFragment(v, d, g)
                                                        : this.loadKey(v, d));
                                            }
                                    }
                                }
                            }),
                            (c.loadFragment = function (e, r, i) {
                                var n,
                                    a = this.fragmentTracker.getState(e);
                                if (((this.fragCurrent = e), a === tt.BACKTRACKED)) {
                                    var s = this.fragmentTracker.getBacktrackData(e);
                                    if (s) return this._handleFragmentLoadProgress(s), void this._handleFragmentLoadComplete(s);
                                    a = tt.NOT_LOADED;
                                }
                                a === tt.NOT_LOADED || a === tt.PARTIAL
                                    ? "initSegment" === e.sn
                                        ? this._loadInitSegment(e)
                                        : this.bitrateTest
                                        ? ((e.bitrateTest = !0), this.log("Fragment " + e.sn + " of level " + e.level + " is being downloaded to test bitrate and will not be buffered"), this._loadBitrateTestFrag(e))
                                        : ((this.startFragRequested = !0), t.prototype.loadFragment.call(this, e, r, i))
                                    : a === tt.APPENDING
                                    ? this.reduceMaxBufferLength(e.duration) && this.fragmentTracker.removeFragment(e)
                                    : 0 === (null === (n = this.media) || void 0 === n ? void 0 : n.buffered.length) && this.fragmentTracker.removeAllFragments();
                            }),
                            (c.getAppendedFrag = function (t) {
                                var e = this.fragmentTracker.getAppendedFrag(t, k.b.MAIN);
                                return e && "fragment" in e ? e.fragment : e;
                            }),
                            (c.getBufferedFrag = function (t) {
                                return this.fragmentTracker.getBufferedFrag(t, k.b.MAIN);
                            }),
                            (c.followingBufferedFrag = function (t) {
                                return t ? this.getBufferedFrag(t.end + 0.5) : null;
                            }),
                            (c.immediateLevelSwitch = function () {
                                this.abortCurrentFrag(), this.flushMainBuffer(0, Number.POSITIVE_INFINITY);
                            }),
                            (c.nextLevelSwitch = function () {
                                var t = this.levels,
                                    e = this.media;
                                if (null != e && e.readyState) {
                                    var r,
                                        i = this.getAppendedFrag(e.currentTime);
                                    if ((i && i.start > 1 && this.flushMainBuffer(0, i.start - 1), !e.paused && t)) {
                                        var n = t[this.hls.nextLoadLevel],
                                            a = this.fragLastKbps;
                                        r = a && this.fragCurrent ? (this.fragCurrent.duration * n.maxBitrate) / (1e3 * a) + 1 : 0;
                                    } else r = 0;
                                    var s = this.getBufferedFrag(e.currentTime + r);
                                    if (s) {
                                        var o = this.followingBufferedFrag(s);
                                        if (o) {
                                            this.abortCurrentFrag();
                                            var l = o.maxStartPTS ? o.maxStartPTS : o.start,
                                                u = o.duration,
                                                h = Math.max(s.end, l + Math.min(Math.max(u - this.config.maxFragLookUpTolerance, 0.5 * u), 0.75 * u));
                                            this.flushMainBuffer(h, Number.POSITIVE_INFINITY);
                                        }
                                    }
                                }
                            }),
                            (c.abortCurrentFrag = function () {
                                var t = this.fragCurrent;
                                (this.fragCurrent = null), null != t && t.loader && t.loader.abort(), this.state === Mt && (this.state = Pt), (this.nextLoadPosition = this.getLoadPosition());
                            }),
                            (c.flushMainBuffer = function (e, r) {
                                t.prototype.flushMainBuffer.call(this, e, r, this.altAudio ? "video" : null);
                            }),
                            (c.onMediaAttached = function (e, r) {
                                t.prototype.onMediaAttached.call(this, e, r);
                                var i = r.media;
                                (this.onvplaying = this.onMediaPlaying.bind(this)),
                                    (this.onvseeked = this.onMediaSeeked.bind(this)),
                                    i.addEventListener("playing", this.onvplaying),
                                    i.addEventListener("seeked", this.onvseeked),
                                    (this.gapController = new te(this.config, i, this.fragmentTracker, this.hls));
                            }),
                            (c.onMediaDetaching = function () {
                                var e = this.media;
                                e && (e.removeEventListener("playing", this.onvplaying), e.removeEventListener("seeked", this.onvseeked), (this.onvplaying = this.onvseeked = null), (this.videoBuffer = null)),
                                    (this.fragPlaying = null),
                                    this.gapController && (this.gapController.destroy(), (this.gapController = null)),
                                    t.prototype.onMediaDetaching.call(this);
                            }),
                            (c.onMediaPlaying = function () {
                                this.tick();
                            }),
                            (c.onMediaSeeked = function () {
                                var t = this.media,
                                    e = t ? t.currentTime : null;
                                Object(n.a)(e) && this.log("Media seeked to " + e.toFixed(3)), this.tick();
                            }),
                            (c.onManifestLoading = function () {
                                this.log("Trigger BUFFER_RESET"),
                                    this.hls.trigger(a.a.BUFFER_RESET, void 0),
                                    this.fragmentTracker.removeAllFragments(),
                                    (this.couldBacktrack = this.stalled = !1),
                                    (this.startPosition = this.lastCurrentTime = 0),
                                    (this.fragPlaying = null);
                            }),
                            (c.onManifestParsed = function (t, e) {
                                var r,
                                    i,
                                    n,
                                    a = !1,
                                    s = !1;
                                e.levels.forEach(function (t) {
                                    (r = t.audioCodec) && (-1 !== r.indexOf("mp4a.40.2") && (a = !0), -1 !== r.indexOf("mp4a.40.5") && (s = !0));
                                }),
                                    (this.audioCodecSwitch = a && s && !("function" == typeof (null == (n = Xt()) || null === (i = n.prototype) || void 0 === i ? void 0 : i.changeType))),
                                    this.audioCodecSwitch && this.log("Both AAC/HE-AAC audio found in levels; declaring level codec as HE-AAC"),
                                    (this.levels = e.levels),
                                    (this.startFragRequested = !1);
                            }),
                            (c.onLevelLoading = function (t, e) {
                                var r = this.levels;
                                if (r && this.state === Pt) {
                                    var i = r[e.level];
                                    (!i.details || (i.details.live && this.levelLastLoaded !== e.level) || this.waitForCdnTuneIn(i.details)) && (this.state = Wt);
                                }
                            }),
                            (c.onLevelLoaded = function (t, e) {
                                var r,
                                    i = this.levels,
                                    n = e.level,
                                    s = e.details,
                                    o = s.totalduration;
                                if (i) {
                                    this.log("Level " + n + " loaded [" + s.startSN + "," + s.endSN + "], cc [" + s.startCC + ", " + s.endCC + "] duration:" + o);
                                    var l = this.fragCurrent;
                                    !l || (this.state !== Ft && this.state !== Nt) || (l.level !== e.level && l.loader && ((this.state = Pt), l.loader.abort()));
                                    var u = i[n],
                                        h = 0;
                                    if (s.live || (null !== (r = u.details) && void 0 !== r && r.live)) {
                                        if ((s.fragments[0] || (s.deltaUpdateFailed = !0), s.deltaUpdateFailed)) return;
                                        h = this.alignPlaylists(s, u.details);
                                    }
                                    if (((u.details = s), (this.levelLastLoaded = n), this.hls.trigger(a.a.LEVEL_UPDATED, { details: s, level: n }), this.state === Wt)) {
                                        if (this.waitForCdnTuneIn(s)) return;
                                        this.state = Pt;
                                    }
                                    this.startFragRequested ? s.live && this.synchronizeToLiveEdge(s) : this.setStartPosition(s, h), this.tick();
                                } else this.warn("Levels were reset while loading level " + n);
                            }),
                            (c._handleFragmentLoadProgress = function (t) {
                                var e,
                                    r = t.frag,
                                    i = t.part,
                                    n = t.payload,
                                    a = this.levels;
                                if (a) {
                                    var s = a[r.level],
                                        o = s.details;
                                    if (o) {
                                        var l = s.videoCodec,
                                            u = o.PTSKnown || !o.live,
                                            h = null === (e = r.initSegment) || void 0 === e ? void 0 : e.data,
                                            d = this._getAudioCodec(s),
                                            c = (this.transmuxer = this.transmuxer || new Zt(this.hls, k.b.MAIN, this._handleTransmuxComplete.bind(this), this._handleTransmuxerFlush.bind(this))),
                                            f = i ? i.index : -1,
                                            g = -1 !== f,
                                            v = new ut(r.level, r.sn, r.stats.chunkCount, n.byteLength, f, g),
                                            p = this.initPTS[r.cc];
                                        c.push(n, h, d, l, r, i, o.totalduration, u, v, p);
                                    } else this.warn("Dropping fragment " + r.sn + " of level " + r.level + " after level details were reset");
                                } else this.warn("Levels were reset while fragment load was in progress. Fragment " + r.sn + " of level " + r.level + " will not be buffered");
                            }),
                            (c.onAudioTrackSwitching = function (t, e) {
                                var r = this.altAudio,
                                    i = !!e.url,
                                    n = e.id;
                                if (!i) {
                                    if (this.mediaBuffer !== this.media) {
                                        this.log("Switching on main audio, use media.buffered to schedule main fragment loading"), (this.mediaBuffer = this.media);
                                        var s = this.fragCurrent;
                                        null != s && s.loader && (this.log("Switching to main audio track, cancel main fragment load"), s.loader.abort()), this.resetTransmuxer(), this.resetLoadingState();
                                    } else this.audioOnly && this.resetTransmuxer();
                                    var o = this.hls;
                                    r && o.trigger(a.a.BUFFER_FLUSHING, { startOffset: 0, endOffset: Number.POSITIVE_INFINITY, type: "audio" }), o.trigger(a.a.AUDIO_TRACK_SWITCHED, { id: n });
                                }
                            }),
                            (c.onAudioTrackSwitched = function (t, e) {
                                var r = e.id,
                                    i = !!this.hls.audioTracks[r].url;
                                if (i) {
                                    var n = this.videoBuffer;
                                    n && this.mediaBuffer !== n && (this.log("Switching on alternate audio, use video.buffered to schedule main fragment loading"), (this.mediaBuffer = n));
                                }
                                (this.altAudio = i), this.tick();
                            }),
                            (c.onBufferCreated = function (t, e) {
                                var r,
                                    i,
                                    n = e.tracks,
                                    a = !1;
                                for (var s in n) {
                                    var o = n[s];
                                    if ("main" === o.id) {
                                        if (((i = s), (r = o), "video" === s)) {
                                            var l = n[s];
                                            l && (this.videoBuffer = l.buffer);
                                        }
                                    } else a = !0;
                                }
                                a && r ? (this.log("Alternate track found, use " + i + ".buffered to schedule main fragment loading"), (this.mediaBuffer = r.buffer)) : (this.mediaBuffer = this.media);
                            }),
                            (c.onFragBuffered = function (t, e) {
                                var r = e.frag,
                                    i = e.part;
                                if (!r || r.type === k.b.MAIN) {
                                    if (this.fragContextChanged(r))
                                        return (
                                            this.warn("Fragment " + r.sn + (i ? " p: " + i.index : "") + " of level " + r.level + " finished buffering, but was aborted. state: " + this.state), void (this.state === Gt && (this.state = Pt))
                                        );
                                    var n = i ? i.stats : r.stats;
                                    (this.fragLastKbps = Math.round((8 * n.total) / (n.buffering.end - n.loading.first))), "initSegment" !== r.sn && (this.fragPrevious = r), this.fragBufferedComplete(r, i);
                                }
                            }),
                            (c.onError = function (t, e) {
                                switch (e.details) {
                                    case s.a.FRAG_LOAD_ERROR:
                                    case s.a.FRAG_LOAD_TIMEOUT:
                                    case s.a.KEY_LOAD_ERROR:
                                    case s.a.KEY_LOAD_TIMEOUT:
                                        this.onFragmentOrKeyLoadError(k.b.MAIN, e);
                                        break;
                                    case s.a.LEVEL_LOAD_ERROR:
                                    case s.a.LEVEL_LOAD_TIMEOUT:
                                        this.state !== Ht && (e.fatal ? (this.warn("" + e.details), (this.state = Ht)) : e.levelRetry || this.state !== Wt || (this.state = Pt));
                                        break;
                                    case s.a.BUFFER_FULL_ERROR:
                                        if ("main" === e.parent && (this.state === Bt || this.state === Gt)) {
                                            var r = !0,
                                                i = this.getFwdBufferInfo(this.media, k.b.MAIN);
                                            i && i.len > 0.5 && (r = !this.reduceMaxBufferLength(i.len)),
                                                r && (this.warn("buffer full error also media.currentTime is not buffered, flush main"), this.immediateLevelSwitch()),
                                                this.resetLoadingState();
                                        }
                                }
                            }),
                            (c.checkBuffer = function () {
                                var t = this.media,
                                    e = this.gapController;
                                if (t && e && t.readyState) {
                                    var r = lt.getBuffered(t);
                                    !this.loadedmetadata && r.length ? ((this.loadedmetadata = !0), this.seekToStartPos()) : e.poll(this.lastCurrentTime), (this.lastCurrentTime = t.currentTime);
                                }
                            }),
                            (c.onFragLoadEmergencyAborted = function () {
                                (this.state = Pt), this.loadedmetadata || ((this.startFragRequested = !1), (this.nextLoadPosition = this.startPosition)), this.tickImmediate();
                            }),
                            (c.onBufferFlushed = function (t, e) {
                                var r = e.type;
                                if (r !== u.a.AUDIO || (this.audioOnly && !this.altAudio)) {
                                    var i = (r === u.a.VIDEO ? this.videoBuffer : this.mediaBuffer) || this.media;
                                    this.afterBufferFlushed(i, r, k.b.MAIN);
                                }
                            }),
                            (c.onLevelsUpdated = function (t, e) {
                                this.levels = e.levels;
                            }),
                            (c.swapAudioCodec = function () {
                                this.audioCodecSwap = !this.audioCodecSwap;
                            }),
                            (c.seekToStartPos = function () {
                                var t = this.media,
                                    e = t.currentTime,
                                    r = this.startPosition;
                                if (r >= 0 && e < r) {
                                    if (t.seeking) return void o.b.log("could not seek to " + r + ", already seeking at " + e);
                                    var i = lt.getBuffered(t),
                                        n = (i.length ? i.start(0) : 0) - r;
                                    n > 0 && (n < this.config.maxBufferHole || n < this.config.maxFragLookUpTolerance) && (o.b.log("adjusting start position by " + n + " to match buffer start"), (r += n), (this.startPosition = r)),
                                        this.log("seek to target start position " + r + " from current time " + e),
                                        (t.currentTime = r);
                                }
                            }),
                            (c._getAudioCodec = function (t) {
                                var e = this.config.defaultAudioCodec || t.audioCodec;
                                return this.audioCodecSwap && e && (this.log("Swapping audio codec"), (e = -1 !== e.indexOf("mp4a.40.5") ? "mp4a.40.2" : "mp4a.40.5")), e;
                            }),
                            (c._loadBitrateTestFrag = function (t) {
                                var e = this;
                                this._doFragLoad(t).then(function (r) {
                                    var i = e.hls;
                                    if (r && !i.nextLoadLevel && !e.fragContextChanged(t)) {
                                        (e.fragLoadError = 0), (e.state = Pt), (e.startFragRequested = !1), (e.bitrateTest = !1);
                                        var n = t.stats;
                                        (n.parsing.start = n.parsing.end = n.buffering.start = n.buffering.end = self.performance.now()), i.trigger(a.a.FRAG_LOADED, r);
                                    }
                                });
                            }),
                            (c._handleTransmuxComplete = function (t) {
                                var e,
                                    r = "main",
                                    i = this.hls,
                                    s = t.remuxResult,
                                    o = t.chunkMeta,
                                    l = this.getCurrentContext(o);
                                if (!l) return this.warn("The loading context changed while buffering fragment " + o.sn + " of level " + o.level + ". This chunk will not be buffered."), void this.resetLiveStartWhenNotLoaded(o.level);
                                var h = l.frag,
                                    d = l.part,
                                    c = l.level,
                                    f = s.video,
                                    g = s.text,
                                    v = s.id3,
                                    p = s.initSegment,
                                    m = this.altAudio ? void 0 : s.audio;
                                if (!this.fragContextChanged(h)) {
                                    if (((this.state = Bt), p)) {
                                        p.tracks && (this._bufferInitSegment(c, p.tracks, h, o), i.trigger(a.a.FRAG_PARSING_INIT_SEGMENT, { frag: h, id: r, tracks: p.tracks }));
                                        var y = p.initPTS,
                                            T = p.timescale;
                                        Object(n.a)(y) && ((this.initPTS[h.cc] = y), i.trigger(a.a.INIT_PTS_FOUND, { frag: h, id: r, initPTS: y, timescale: T }));
                                    }
                                    if (f && !1 !== s.independent) {
                                        if (c.details) {
                                            var b = f.startPTS,
                                                E = f.endPTS,
                                                S = f.startDTS,
                                                L = f.endDTS;
                                            if (d) d.elementaryStreams[f.type] = { startPTS: b, endPTS: E, startDTS: S, endDTS: L };
                                            else if ((f.firstKeyFrame && f.independent && (this.couldBacktrack = !0), f.dropped && f.independent)) {
                                                if (this.getLoadPosition() + this.config.maxBufferHole < b) return void this.backtrack(h);
                                                h.setElementaryStreamInfo(f.type, h.start, E, h.start, L, !0);
                                            }
                                            h.setElementaryStreamInfo(f.type, b, E, S, L), this.bufferFragmentData(f, h, d, o);
                                        }
                                    } else if (!1 === s.independent) return void this.backtrack(h);
                                    if (m) {
                                        var A = m.startPTS,
                                            R = m.endPTS,
                                            D = m.startDTS,
                                            k = m.endDTS;
                                        d && (d.elementaryStreams[u.a.AUDIO] = { startPTS: A, endPTS: R, startDTS: D, endDTS: k }), h.setElementaryStreamInfo(u.a.AUDIO, A, R, D, k), this.bufferFragmentData(m, h, d, o);
                                    }
                                    if (null != v && null !== (e = v.samples) && void 0 !== e && e.length) {
                                        var _ = { frag: h, id: r, samples: v.samples };
                                        i.trigger(a.a.FRAG_PARSING_METADATA, _);
                                    }
                                    if (g) {
                                        var I = { frag: h, id: r, samples: g.samples };
                                        i.trigger(a.a.FRAG_PARSING_USERDATA, I);
                                    }
                                }
                            }),
                            (c._bufferInitSegment = function (t, e, r, i) {
                                var n = this;
                                if (this.state === Bt) {
                                    (this.audioOnly = !!e.audio && !e.video), this.altAudio && !this.audioOnly && delete e.audio;
                                    var s = e.audio,
                                        o = e.video,
                                        l = e.audiovideo;
                                    if (s) {
                                        var u = t.audioCodec,
                                            h = navigator.userAgent.toLowerCase();
                                        this.audioCodecSwitch && (u && (u = -1 !== u.indexOf("mp4a.40.5") ? "mp4a.40.2" : "mp4a.40.5"), 1 !== s.metadata.channelCount && -1 === h.indexOf("firefox") && (u = "mp4a.40.5")),
                                            -1 !== h.indexOf("android") && "audio/mpeg" !== s.container && ((u = "mp4a.40.2"), this.log("Android: force audio codec to " + u)),
                                            t.audioCodec && t.audioCodec !== u && this.log('Swapping manifest audio codec "' + t.audioCodec + '" for "' + u + '"'),
                                            (s.levelCodec = u),
                                            (s.id = "main"),
                                            this.log("Init audio buffer, container:" + s.container + ", codecs[selected/level/parsed]=[" + (u || "") + "/" + (t.audioCodec || "") + "/" + s.codec + "]");
                                    }
                                    o && ((o.levelCodec = t.videoCodec), (o.id = "main"), this.log("Init video buffer, container:" + o.container + ", codecs[level/parsed]=[" + (t.videoCodec || "") + "/" + o.codec + "]")),
                                        l && this.log("Init audiovideo buffer, container:" + l.container + ", codecs[level/parsed]=[" + (t.attrs.CODECS || "") + "/" + l.codec + "]"),
                                        this.hls.trigger(a.a.BUFFER_CODECS, e),
                                        Object.keys(e).forEach(function (t) {
                                            var s = e[t].initSegment;
                                            null != s && s.byteLength && n.hls.trigger(a.a.BUFFER_APPENDING, { type: t, data: s, frag: r, part: null, chunkMeta: i, parent: r.type });
                                        }),
                                        this.tick();
                                }
                            }),
                            (c.backtrack = function (t) {
                                (this.couldBacktrack = !0), this.resetTransmuxer(), this.flushBufferGap(t);
                                var e = this.fragmentTracker.backtrack(t);
                                (this.fragPrevious = null), (this.nextLoadPosition = t.start), e ? this.resetFragmentLoading(t) : (this.state = Kt);
                            }),
                            (c.checkFragmentChanged = function () {
                                var t = this.media,
                                    e = null;
                                if (t && t.readyState > 1 && !1 === t.seeking) {
                                    var r = t.currentTime;
                                    if ((lt.isBuffered(t, r) ? (e = this.getAppendedFrag(r)) : lt.isBuffered(t, r + 0.1) && (e = this.getAppendedFrag(r + 0.1)), e)) {
                                        var i = this.fragPlaying,
                                            n = e.level;
                                        (i && e.sn === i.sn && i.level === n && e.urlId === i.urlId) ||
                                            (this.hls.trigger(a.a.FRAG_CHANGED, { frag: e }), (i && i.level === n) || this.hls.trigger(a.a.LEVEL_SWITCHED, { level: n }), (this.fragPlaying = e));
                                    }
                                }
                            }),
                            (l = i),
                            (h = [
                                {
                                    key: "nextLevel",
                                    get: function () {
                                        var t = this.nextBufferedFrag;
                                        return t ? t.level : -1;
                                    },
                                },
                                {
                                    key: "currentLevel",
                                    get: function () {
                                        var t = this.media;
                                        if (t) {
                                            var e = this.getAppendedFrag(t.currentTime);
                                            if (e) return e.level;
                                        }
                                        return -1;
                                    },
                                },
                                {
                                    key: "nextBufferedFrag",
                                    get: function () {
                                        var t = this.media;
                                        if (t) {
                                            var e = this.getAppendedFrag(t.currentTime);
                                            return this.followingBufferedFrag(e);
                                        }
                                        return null;
                                    },
                                },
                                {
                                    key: "forceStartLoad",
                                    get: function () {
                                        return this._forceStartLoad;
                                    },
                                },
                            ]) && ee(l.prototype, h),
                            d && ee(l, d),
                            i
                        );
                    })(Yt),
                    ne = (function () {
                        function t(t, e, r) {
                            void 0 === e && (e = 0),
                                void 0 === r && (r = 0),
                                (this.halfLife = void 0),
                                (this.alpha_ = void 0),
                                (this.estimate_ = void 0),
                                (this.totalWeight_ = void 0),
                                (this.halfLife = t),
                                (this.alpha_ = t ? Math.exp(Math.log(0.5) / t) : 0),
                                (this.estimate_ = e),
                                (this.totalWeight_ = r);
                        }
                        var e = t.prototype;
                        return (
                            (e.sample = function (t, e) {
                                var r = Math.pow(this.alpha_, t);
                                (this.estimate_ = e * (1 - r) + r * this.estimate_), (this.totalWeight_ += t);
                            }),
                            (e.getTotalWeight = function () {
                                return this.totalWeight_;
                            }),
                            (e.getEstimate = function () {
                                if (this.alpha_) {
                                    var t = 1 - Math.pow(this.alpha_, this.totalWeight_);
                                    if (t) return this.estimate_ / t;
                                }
                                return this.estimate_;
                            }),
                            t
                        );
                    })(),
                    ae = (function () {
                        function t(t, e, r) {
                            (this.defaultEstimate_ = void 0),
                                (this.minWeight_ = void 0),
                                (this.minDelayMs_ = void 0),
                                (this.slow_ = void 0),
                                (this.fast_ = void 0),
                                (this.defaultEstimate_ = r),
                                (this.minWeight_ = 0.001),
                                (this.minDelayMs_ = 50),
                                (this.slow_ = new ne(t)),
                                (this.fast_ = new ne(e));
                        }
                        var e = t.prototype;
                        return (
                            (e.update = function (t, e) {
                                var r = this.slow_,
                                    i = this.fast_;
                                this.slow_.halfLife !== t && (this.slow_ = new ne(t, r.getEstimate(), r.getTotalWeight())), this.fast_.halfLife !== e && (this.fast_ = new ne(e, i.getEstimate(), i.getTotalWeight()));
                            }),
                            (e.sample = function (t, e) {
                                var r = (t = Math.max(t, this.minDelayMs_)) / 1e3,
                                    i = (8 * e) / r;
                                this.fast_.sample(r, i), this.slow_.sample(r, i);
                            }),
                            (e.canEstimate = function () {
                                var t = this.fast_;
                                return t && t.getTotalWeight() >= this.minWeight_;
                            }),
                            (e.getEstimate = function () {
                                return this.canEstimate() ? Math.min(this.fast_.getEstimate(), this.slow_.getEstimate()) : this.defaultEstimate_;
                            }),
                            (e.destroy = function () {}),
                            t
                        );
                    })();
                function se(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var oe = (function () {
                        function t(t) {
                            (this.hls = void 0),
                                (this.lastLoadedFragLevel = 0),
                                (this._nextAutoLevel = -1),
                                (this.timer = void 0),
                                (this.onCheck = this._abandonRulesCheck.bind(this)),
                                (this.fragCurrent = null),
                                (this.partCurrent = null),
                                (this.bitrateTestDelay = 0),
                                (this.bwEstimator = void 0),
                                (this.hls = t);
                            var e = t.config;
                            (this.bwEstimator = new ae(e.abrEwmaSlowVoD, e.abrEwmaFastVoD, e.abrEwmaDefaultEstimate)), this.registerListeners();
                        }
                        var e,
                            r,
                            i,
                            l = t.prototype;
                        return (
                            (l.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.FRAG_LOADING, this.onFragLoading, this),
                                    t.on(a.a.FRAG_LOADED, this.onFragLoaded, this),
                                    t.on(a.a.FRAG_BUFFERED, this.onFragBuffered, this),
                                    t.on(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.on(a.a.ERROR, this.onError, this);
                            }),
                            (l.unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.FRAG_LOADING, this.onFragLoading, this),
                                    t.off(a.a.FRAG_LOADED, this.onFragLoaded, this),
                                    t.off(a.a.FRAG_BUFFERED, this.onFragBuffered, this),
                                    t.off(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                    t.off(a.a.ERROR, this.onError, this);
                            }),
                            (l.destroy = function () {
                                this.unregisterListeners(), this.clearTimer(), (this.hls = this.onCheck = null), (this.fragCurrent = this.partCurrent = null);
                            }),
                            (l.onFragLoading = function (t, e) {
                                var r,
                                    i = e.frag;
                                i.type === k.b.MAIN && (this.timer || ((this.fragCurrent = i), (this.partCurrent = null != (r = e.part) ? r : null), (this.timer = self.setInterval(this.onCheck, 100))));
                            }),
                            (l.onLevelLoaded = function (t, e) {
                                var r = this.hls.config;
                                e.details.live ? this.bwEstimator.update(r.abrEwmaSlowLive, r.abrEwmaFastLive) : this.bwEstimator.update(r.abrEwmaSlowVoD, r.abrEwmaFastVoD);
                            }),
                            (l._abandonRulesCheck = function () {
                                var t = this.fragCurrent,
                                    e = this.partCurrent,
                                    r = this.hls,
                                    i = r.autoLevelEnabled,
                                    s = r.config,
                                    l = r.media;
                                if (t && l) {
                                    var u = e ? e.stats : t.stats,
                                        h = e ? e.duration : t.duration;
                                    if (u.aborted) return o.b.warn("frag loader destroy or aborted, disarm abandonRules"), this.clearTimer(), void (this._nextAutoLevel = -1);
                                    if (i && !l.paused && l.playbackRate && l.readyState) {
                                        var d = performance.now() - u.loading.start,
                                            c = Math.abs(l.playbackRate);
                                        if (!(d <= (500 * h) / c)) {
                                            var f = r.levels,
                                                g = r.minAutoLevel,
                                                v = f[t.level],
                                                p = u.total || Math.max(u.loaded, Math.round((h * v.maxBitrate) / 8)),
                                                m = Math.max(1, u.bwEstimate ? u.bwEstimate / 8 : (1e3 * u.loaded) / d),
                                                y = (p - u.loaded) / m,
                                                T = l.currentTime,
                                                b = (lt.bufferInfo(l, T, s.maxBufferHole).end - T) / c;
                                            if (!(b >= (2 * h) / c || y <= b)) {
                                                var E,
                                                    S = Number.POSITIVE_INFINITY;
                                                for (E = t.level - 1; E > g; E--) {
                                                    if ((S = (h * f[E].maxBitrate) / (6.4 * m)) < b) break;
                                                }
                                                if (!(S >= y)) {
                                                    var L = this.bwEstimator.getEstimate();
                                                    o.b.warn(
                                                        "Fragment " +
                                                            t.sn +
                                                            (e ? " part " + e.index : "") +
                                                            " of level " +
                                                            t.level +
                                                            " is loading too slowly and will cause an underbuffer; aborting and switching to level " +
                                                            E +
                                                            "\n      Current BW estimate: " +
                                                            (Object(n.a)(L) ? (L / 1024).toFixed(3) : "Unknown") +
                                                            " Kb/s\n      Estimated load time for current fragment: " +
                                                            y.toFixed(3) +
                                                            " s\n      Estimated load time for the next fragment: " +
                                                            S.toFixed(3) +
                                                            " s\n      Time to underbuffer: " +
                                                            b.toFixed(3) +
                                                            " s"
                                                    ),
                                                        (r.nextLoadLevel = E),
                                                        this.bwEstimator.sample(d, u.loaded),
                                                        this.clearTimer(),
                                                        t.loader && ((this.fragCurrent = this.partCurrent = null), t.loader.abort()),
                                                        r.trigger(a.a.FRAG_LOAD_EMERGENCY_ABORTED, { frag: t, part: e, stats: u });
                                                }
                                            }
                                        }
                                    }
                                }
                            }),
                            (l.onFragLoaded = function (t, e) {
                                var r = e.frag,
                                    i = e.part;
                                if (r.type === k.b.MAIN && Object(n.a)(r.sn)) {
                                    var s = i ? i.stats : r.stats,
                                        o = i ? i.duration : r.duration;
                                    if ((this.clearTimer(), (this.lastLoadedFragLevel = r.level), (this._nextAutoLevel = -1), this.hls.config.abrMaxWithRealBitrate)) {
                                        var l = this.hls.levels[r.level],
                                            u = (l.loaded ? l.loaded.bytes : 0) + s.loaded,
                                            h = (l.loaded ? l.loaded.duration : 0) + o;
                                        (l.loaded = { bytes: u, duration: h }), (l.realBitrate = Math.round((8 * u) / h));
                                    }
                                    if (r.bitrateTest) {
                                        var d = { stats: s, frag: r, part: i, id: r.type };
                                        this.onFragBuffered(a.a.FRAG_BUFFERED, d), (r.bitrateTest = !1);
                                    }
                                }
                            }),
                            (l.onFragBuffered = function (t, e) {
                                var r = e.frag,
                                    i = e.part,
                                    n = i ? i.stats : r.stats;
                                if (!n.aborted && r.type === k.b.MAIN && "initSegment" !== r.sn) {
                                    var a = n.parsing.end - n.loading.start;
                                    this.bwEstimator.sample(a, n.loaded), (n.bwEstimate = this.bwEstimator.getEstimate()), r.bitrateTest ? (this.bitrateTestDelay = a / 1e3) : (this.bitrateTestDelay = 0);
                                }
                            }),
                            (l.onError = function (t, e) {
                                switch (e.details) {
                                    case s.a.FRAG_LOAD_ERROR:
                                    case s.a.FRAG_LOAD_TIMEOUT:
                                        this.clearTimer();
                                }
                            }),
                            (l.clearTimer = function () {
                                self.clearInterval(this.timer), (this.timer = void 0);
                            }),
                            (l.getNextABRAutoLevel = function () {
                                var t = this.fragCurrent,
                                    e = this.partCurrent,
                                    r = this.hls,
                                    i = r.maxAutoLevel,
                                    n = r.config,
                                    a = r.minAutoLevel,
                                    s = r.media,
                                    l = e ? e.duration : t ? t.duration : 0,
                                    u = s ? s.currentTime : 0,
                                    h = s && 0 !== s.playbackRate ? Math.abs(s.playbackRate) : 1,
                                    d = this.bwEstimator ? this.bwEstimator.getEstimate() : n.abrEwmaDefaultEstimate,
                                    c = (lt.bufferInfo(s, u, n.maxBufferHole).end - u) / h,
                                    f = this.findBestLevel(d, a, i, c, n.abrBandWidthFactor, n.abrBandWidthUpFactor);
                                if (f >= 0) return f;
                                o.b.trace((c ? "rebuffering expected" : "buffer is empty") + ", finding optimal quality level");
                                var g = l ? Math.min(l, n.maxStarvationDelay) : n.maxStarvationDelay,
                                    v = n.abrBandWidthFactor,
                                    p = n.abrBandWidthUpFactor;
                                if (!c) {
                                    var m = this.bitrateTestDelay;
                                    if (m)
                                        (g = (l ? Math.min(l, n.maxLoadingDelay) : n.maxLoadingDelay) - m),
                                            o.b.trace("bitrate test took " + Math.round(1e3 * m) + "ms, set first fragment max fetchDuration to " + Math.round(1e3 * g) + " ms"),
                                            (v = p = 1);
                                }
                                return (f = this.findBestLevel(d, a, i, c + g, v, p)), Math.max(f, 0);
                            }),
                            (l.findBestLevel = function (t, e, r, i, n, a) {
                                for (
                                    var s,
                                        l = this.fragCurrent,
                                        u = this.partCurrent,
                                        h = this.lastLoadedFragLevel,
                                        d = this.hls.levels,
                                        c = d[h],
                                        f = !(null == c || null === (s = c.details) || void 0 === s || !s.live),
                                        g = null == c ? void 0 : c.codecSet,
                                        v = u ? u.duration : l ? l.duration : 0,
                                        p = r;
                                    p >= e;
                                    p--
                                ) {
                                    var m = d[p];
                                    if (m && (!g || m.codecSet === g)) {
                                        var y = m.details,
                                            T = (u ? (null == y ? void 0 : y.partTarget) : null == y ? void 0 : y.averagetargetduration) || v,
                                            b = void 0;
                                        b = p <= h ? n * t : a * t;
                                        var E = d[p].maxBitrate,
                                            S = (E * T) / b;
                                        if (
                                            (o.b.trace("level/adjustedbw/bitrate/avgDuration/maxFetchDuration/fetchDuration: " + p + "/" + Math.round(b) + "/" + E + "/" + T + "/" + i + "/" + S),
                                            b > E && (!S || (f && !this.bitrateTestDelay) || S < i))
                                        )
                                            return p;
                                    }
                                }
                                return -1;
                            }),
                            (e = t),
                            (r = [
                                {
                                    key: "nextAutoLevel",
                                    get: function () {
                                        var t = this._nextAutoLevel,
                                            e = this.bwEstimator;
                                        if (!(-1 === t || (e && e.canEstimate()))) return t;
                                        var r = this.getNextABRAutoLevel();
                                        return -1 !== t && (r = Math.min(t, r)), r;
                                    },
                                    set: function (t) {
                                        this._nextAutoLevel = t;
                                    },
                                },
                            ]) && se(e.prototype, r),
                            i && se(e, i),
                            t
                        );
                    })(),
                    le = r(13);
                function ue() {
                    return (ue =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                function he(t, e) {
                    return (he =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var de = (function (t) {
                    var e, r;
                    function i(e, r) {
                        var i;
                        return (
                            ((i = t.call(this, e, r, "[audio-stream-controller]") || this).videoBuffer = null),
                            (i.videoTrackCC = -1),
                            (i.waitingVideoCC = -1),
                            (i.audioSwitch = !1),
                            (i.trackId = -1),
                            (i.waitingData = null),
                            (i.mainDetails = null),
                            (i.bufferFlushed = !1),
                            i._registerListeners(),
                            i
                        );
                    }
                    (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), he(e, r);
                    var l = i.prototype;
                    return (
                        (l.onHandlerDestroying = function () {
                            this._unregisterListeners(), (this.mainDetails = null);
                        }),
                        (l._registerListeners = function () {
                            var t = this.hls;
                            t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.on(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                t.on(a.a.AUDIO_TRACKS_UPDATED, this.onAudioTracksUpdated, this),
                                t.on(a.a.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this),
                                t.on(a.a.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this),
                                t.on(a.a.ERROR, this.onError, this),
                                t.on(a.a.BUFFER_RESET, this.onBufferReset, this),
                                t.on(a.a.BUFFER_CREATED, this.onBufferCreated, this),
                                t.on(a.a.BUFFER_FLUSHED, this.onBufferFlushed, this),
                                t.on(a.a.INIT_PTS_FOUND, this.onInitPtsFound, this),
                                t.on(a.a.FRAG_BUFFERED, this.onFragBuffered, this);
                        }),
                        (l._unregisterListeners = function () {
                            var t = this.hls;
                            t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.off(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                t.off(a.a.AUDIO_TRACKS_UPDATED, this.onAudioTracksUpdated, this),
                                t.off(a.a.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this),
                                t.off(a.a.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this),
                                t.off(a.a.ERROR, this.onError, this),
                                t.off(a.a.BUFFER_RESET, this.onBufferReset, this),
                                t.off(a.a.BUFFER_CREATED, this.onBufferCreated, this),
                                t.off(a.a.BUFFER_FLUSHED, this.onBufferFlushed, this),
                                t.off(a.a.INIT_PTS_FOUND, this.onInitPtsFound, this),
                                t.off(a.a.FRAG_BUFFERED, this.onFragBuffered, this);
                        }),
                        (l.onInitPtsFound = function (t, e) {
                            var r = e.frag,
                                i = e.id,
                                n = e.initPTS;
                            if ("main" === i) {
                                var a = r.cc;
                                (this.initPTS[r.cc] = n), this.log("InitPTS for cc: " + a + " found from main: " + n), (this.videoTrackCC = a), this.state === Vt && this.tick();
                            }
                        }),
                        (l.startLoad = function (t) {
                            if (!this.levels) return (this.startPosition = t), void (this.state = xt);
                            var e = this.lastCurrentTime;
                            this.stopLoad(),
                                this.setInterval(100),
                                (this.fragLoadError = 0),
                                e > 0 && -1 === t ? (this.log("Override startPosition with lastCurrentTime @" + e.toFixed(3)), (this.state = Pt)) : ((this.loadedmetadata = !1), (this.state = Ut)),
                                (this.nextLoadPosition = this.startPosition = this.lastCurrentTime = t),
                                this.tick();
                        }),
                        (l.doTick = function () {
                            switch (this.state) {
                                case Pt:
                                    this.doTickIdle();
                                    break;
                                case Ut:
                                    var e,
                                        r = this.levels,
                                        i = this.trackId,
                                        n = null == r || null === (e = r[i]) || void 0 === e ? void 0 : e.details;
                                    if (n) {
                                        if (this.waitForCdnTuneIn(n)) break;
                                        this.state = Vt;
                                    }
                                    break;
                                case Nt:
                                    var a,
                                        s = performance.now(),
                                        l = this.retryDate;
                                    (!l || s >= l || (null !== (a = this.media) && void 0 !== a && a.seeking)) && (this.log("RetryDate reached, switch back to IDLE state"), (this.state = Pt));
                                    break;
                                case Vt:
                                    var u = this.waitingData;
                                    if (u) {
                                        var h = u.frag,
                                            d = u.part,
                                            c = u.cache,
                                            f = u.complete;
                                        if (void 0 !== this.initPTS[h.cc]) {
                                            (this.waitingData = null), (this.waitingVideoCC = -1), (this.state = Ft);
                                            var g = { frag: h, part: d, payload: c.flush(), networkDetails: null };
                                            this._handleFragmentLoadProgress(g), f && t.prototype._handleFragmentLoadComplete.call(this, g);
                                        } else if (this.videoTrackCC !== this.waitingVideoCC) o.b.log("Waiting fragment cc (" + h.cc + ") cancelled because video is at cc " + this.videoTrackCC), this.clearWaitingFragment();
                                        else {
                                            var v = this.getLoadPosition(),
                                                p = lt.bufferInfo(this.mediaBuffer, v, this.config.maxBufferHole);
                                            yt(p.end, this.config.maxFragLookUpTolerance, h) < 0 &&
                                                (o.b.log("Waiting fragment cc (" + h.cc + ") @ " + h.start + " cancelled because another fragment at " + p.end + " is needed"), this.clearWaitingFragment());
                                        }
                                    } else this.state = Pt;
                            }
                            this.onTickEnd();
                        }),
                        (l.clearWaitingFragment = function () {
                            var t = this.waitingData;
                            t && (this.fragmentTracker.removeFragment(t.frag), (this.waitingData = null), (this.waitingVideoCC = -1), (this.state = Pt));
                        }),
                        (l.onTickEnd = function () {
                            var t = this.media;
                            if (t && t.readyState) {
                                var e = (this.mediaBuffer ? this.mediaBuffer : t).buffered;
                                !this.loadedmetadata && e.length && (this.loadedmetadata = !0), (this.lastCurrentTime = t.currentTime);
                            }
                        }),
                        (l.doTickIdle = function () {
                            var t,
                                e,
                                r = this.hls,
                                i = this.levels,
                                n = this.media,
                                s = this.trackId,
                                o = r.config;
                            if (i && i[s] && (n || (!this.startFragRequested && o.startFragPrefetch))) {
                                var l = i[s].details;
                                if (!l || (l.live && this.levelLastLoaded !== s) || this.waitForCdnTuneIn(l)) this.state = Ut;
                                else {
                                    this.bufferFlushed && ((this.bufferFlushed = !1), this.afterBufferFlushed(this.mediaBuffer ? this.mediaBuffer : this.media, u.a.AUDIO, k.b.AUDIO));
                                    var h = this.getFwdBufferInfo(this.mediaBuffer ? this.mediaBuffer : this.media, k.b.AUDIO);
                                    if (null !== h) {
                                        var d = h.len,
                                            c = this.getMaxBufferLength(),
                                            f = this.audioSwitch;
                                        if (!(d >= c) || f) {
                                            if (!f && this._streamEnded(h, l)) return r.trigger(a.a.BUFFER_EOS, { type: "audio" }), void (this.state = jt);
                                            var g = l.fragments[0].start,
                                                v = h.end;
                                            if (f) {
                                                var p = this.getLoadPosition();
                                                (v = p), l.PTSKnown && p < g && (h.end > g || h.nextStart) && (this.log("Alt audio track ahead of main track, seek to start of alt audio track"), (n.currentTime = g + 0.05));
                                            }
                                            var m = this.getNextFragment(v, l);
                                            m
                                                ? "identity" !== (null === (t = m.decryptdata) || void 0 === t ? void 0 : t.keyFormat) || (null !== (e = m.decryptdata) && void 0 !== e && e.key)
                                                    ? this.loadFragment(m, l, v)
                                                    : this.loadKey(m, l)
                                                : (this.bufferFlushed = !0);
                                        }
                                    }
                                }
                            }
                        }),
                        (l.getMaxBufferLength = function () {
                            var e = t.prototype.getMaxBufferLength.call(this),
                                r = this.getFwdBufferInfo(this.videoBuffer ? this.videoBuffer : this.media, k.b.MAIN);
                            return null === r ? e : Math.max(e, r.len);
                        }),
                        (l.onMediaDetaching = function () {
                            (this.videoBuffer = null), t.prototype.onMediaDetaching.call(this);
                        }),
                        (l.onAudioTracksUpdated = function (t, e) {
                            var r = e.audioTracks;
                            this.resetTransmuxer(),
                                (this.levels = r.map(function (t) {
                                    return new j(t);
                                }));
                        }),
                        (l.onAudioTrackSwitching = function (t, e) {
                            var r = !!e.url;
                            this.trackId = e.id;
                            var i = this.fragCurrent;
                            null != i && i.loader && i.loader.abort(),
                                (this.fragCurrent = null),
                                this.clearWaitingFragment(),
                                r ? this.setInterval(100) : this.resetTransmuxer(),
                                r ? ((this.audioSwitch = !0), (this.state = Pt)) : (this.state = xt),
                                this.tick();
                        }),
                        (l.onManifestLoading = function () {
                            (this.mainDetails = null), this.fragmentTracker.removeAllFragments(), (this.startPosition = this.lastCurrentTime = 0), (this.bufferFlushed = !1);
                        }),
                        (l.onLevelLoaded = function (t, e) {
                            this.mainDetails = e.details;
                        }),
                        (l.onAudioTrackLoaded = function (t, e) {
                            var r,
                                i = this.levels,
                                n = e.details,
                                a = e.id;
                            if (i) {
                                this.log("Track " + a + " loaded [" + n.startSN + "," + n.endSN + "],duration:" + n.totalduration);
                                var s = i[a],
                                    o = 0;
                                if (n.live || (null !== (r = s.details) && void 0 !== r && r.live)) {
                                    var l = this.mainDetails;
                                    if ((n.fragments[0] || (n.deltaUpdateFailed = !0), n.deltaUpdateFailed || !l)) return;
                                    !s.details && n.hasProgramDateTime && l.hasProgramDateTime ? (gt(n, l), (o = n.fragments[0].start)) : (o = this.alignPlaylists(n, s.details));
                                }
                                (s.details = n),
                                    (this.levelLastLoaded = a),
                                    this.startFragRequested || (!this.mainDetails && n.live) || this.setStartPosition(s.details, o),
                                    this.state !== Ut || this.waitForCdnTuneIn(n) || (this.state = Pt),
                                    this.tick();
                            } else this.warn("Audio tracks were reset while loading level " + a);
                        }),
                        (l._handleFragmentLoadProgress = function (t) {
                            var e,
                                r = t.frag,
                                i = t.part,
                                n = t.payload,
                                a = this.config,
                                s = this.trackId,
                                l = this.levels;
                            if (l) {
                                var u = l[s],
                                    h = u.details,
                                    d = a.defaultAudioCodec || u.audioCodec || "mp4a.40.2",
                                    c = this.transmuxer;
                                c || (c = this.transmuxer = new Zt(this.hls, k.b.AUDIO, this._handleTransmuxComplete.bind(this), this._handleTransmuxerFlush.bind(this)));
                                var f = this.initPTS[r.cc],
                                    g = null === (e = r.initSegment) || void 0 === e ? void 0 : e.data;
                                if (void 0 !== f) {
                                    var v = i ? i.index : -1,
                                        p = -1 !== v,
                                        m = new ut(r.level, r.sn, r.stats.chunkCount, n.byteLength, v, p);
                                    c.push(n, g, d, "", r, i, h.totalduration, !1, m, f);
                                } else {
                                    o.b.log("Unknown video PTS for cc " + r.cc + ", waiting for video PTS before demuxing audio frag " + r.sn + " of [" + h.startSN + " ," + h.endSN + "],track " + s),
                                        (this.waitingData = this.waitingData || { frag: r, part: i, cache: new le.a(), complete: !1 }).cache.push(new Uint8Array(n)),
                                        (this.waitingVideoCC = this.videoTrackCC),
                                        (this.state = Vt);
                                }
                            } else this.warn("Audio tracks were reset while fragment load was in progress. Fragment " + r.sn + " of level " + r.level + " will not be buffered");
                        }),
                        (l._handleFragmentLoadComplete = function (e) {
                            this.waitingData ? (this.waitingData.complete = !0) : t.prototype._handleFragmentLoadComplete.call(this, e);
                        }),
                        (l.onBufferReset = function () {
                            (this.mediaBuffer = this.videoBuffer = null), (this.loadedmetadata = !1);
                        }),
                        (l.onBufferCreated = function (t, e) {
                            var r = e.tracks.audio;
                            r && (this.mediaBuffer = r.buffer), e.tracks.video && (this.videoBuffer = e.tracks.video.buffer);
                        }),
                        (l.onFragBuffered = function (t, e) {
                            var r = e.frag,
                                i = e.part;
                            r.type === k.b.AUDIO &&
                                (this.fragContextChanged(r)
                                    ? this.warn("Fragment " + r.sn + (i ? " p: " + i.index : "") + " of level " + r.level + " finished buffering, but was aborted. state: " + this.state + ", audioSwitch: " + this.audioSwitch)
                                    : ("initSegment" !== r.sn && ((this.fragPrevious = r), this.audioSwitch && ((this.audioSwitch = !1), this.hls.trigger(a.a.AUDIO_TRACK_SWITCHED, { id: this.trackId }))), this.fragBufferedComplete(r, i)));
                        }),
                        (l.onError = function (e, r) {
                            switch (r.details) {
                                case s.a.FRAG_LOAD_ERROR:
                                case s.a.FRAG_LOAD_TIMEOUT:
                                case s.a.KEY_LOAD_ERROR:
                                case s.a.KEY_LOAD_TIMEOUT:
                                    this.onFragmentOrKeyLoadError(k.b.AUDIO, r);
                                    break;
                                case s.a.AUDIO_TRACK_LOAD_ERROR:
                                case s.a.AUDIO_TRACK_LOAD_TIMEOUT:
                                    this.state !== Ht && this.state !== xt && ((this.state = r.fatal ? Ht : Pt), this.warn(r.details + " while loading frag, switching to " + this.state + " state"));
                                    break;
                                case s.a.BUFFER_FULL_ERROR:
                                    if ("audio" === r.parent && (this.state === Bt || this.state === Gt)) {
                                        var i = !0,
                                            n = this.getFwdBufferInfo(this.mediaBuffer, k.b.AUDIO);
                                        n && n.len > 0.5 && (i = !this.reduceMaxBufferLength(n.len)),
                                            i &&
                                                (this.warn("Buffer full error also media.currentTime is not buffered, flush audio buffer"),
                                                (this.fragCurrent = null),
                                                t.prototype.flushMainBuffer.call(this, 0, Number.POSITIVE_INFINITY, "audio")),
                                            this.resetLoadingState();
                                    }
                            }
                        }),
                        (l.onBufferFlushed = function (t, e) {
                            e.type === u.a.AUDIO && (this.bufferFlushed = !0);
                        }),
                        (l._handleTransmuxComplete = function (t) {
                            var e,
                                r = "audio",
                                i = this.hls,
                                n = t.remuxResult,
                                s = t.chunkMeta,
                                o = this.getCurrentContext(s);
                            if (!o) return this.warn("The loading context changed while buffering fragment " + s.sn + " of level " + s.level + ". This chunk will not be buffered."), void this.resetLiveStartWhenNotLoaded(s.level);
                            var l = o.frag,
                                h = o.part,
                                d = n.audio,
                                c = n.text,
                                f = n.id3,
                                g = n.initSegment;
                            if (!this.fragContextChanged(l)) {
                                if (
                                    ((this.state = Bt),
                                    this.audioSwitch && d && this.completeAudioSwitch(),
                                    null != g && g.tracks && (this._bufferInitSegment(g.tracks, l, s), i.trigger(a.a.FRAG_PARSING_INIT_SEGMENT, { frag: l, id: r, tracks: g.tracks })),
                                    d)
                                ) {
                                    var v = d.startPTS,
                                        p = d.endPTS,
                                        m = d.startDTS,
                                        y = d.endDTS;
                                    h && (h.elementaryStreams[u.a.AUDIO] = { startPTS: v, endPTS: p, startDTS: m, endDTS: y }), l.setElementaryStreamInfo(u.a.AUDIO, v, p, m, y), this.bufferFragmentData(d, l, h, s);
                                }
                                if (null != f && null !== (e = f.samples) && void 0 !== e && e.length) {
                                    var T = ue({ frag: l, id: r }, f);
                                    i.trigger(a.a.FRAG_PARSING_METADATA, T);
                                }
                                if (c) {
                                    var b = ue({ frag: l, id: r }, c);
                                    i.trigger(a.a.FRAG_PARSING_USERDATA, b);
                                }
                            }
                        }),
                        (l._bufferInitSegment = function (t, e, r) {
                            if (this.state === Bt) {
                                t.video && delete t.video;
                                var i = t.audio;
                                if (i) {
                                    (i.levelCodec = i.codec), (i.id = "audio"), this.log("Init audio buffer, container:" + i.container + ", codecs[parsed]=[" + i.codec + "]"), this.hls.trigger(a.a.BUFFER_CODECS, t);
                                    var n = i.initSegment;
                                    if (null != n && n.byteLength) {
                                        var s = { type: "audio", frag: e, part: null, chunkMeta: r, parent: e.type, data: n };
                                        this.hls.trigger(a.a.BUFFER_APPENDING, s);
                                    }
                                    this.tick();
                                }
                            }
                        }),
                        (l.loadFragment = function (e, r, i) {
                            var a = this.fragmentTracker.getState(e);
                            (this.fragCurrent = e),
                                (this.audioSwitch || a === tt.NOT_LOADED || a === tt.PARTIAL) &&
                                    ("initSegment" === e.sn
                                        ? this._loadInitSegment(e)
                                        : r.live && !Object(n.a)(this.initPTS[e.cc])
                                        ? (this.log("Waiting for video PTS in continuity counter " + e.cc + " of live stream before loading audio fragment " + e.sn + " of level " + this.trackId), (this.state = Vt))
                                        : ((this.startFragRequested = !0), t.prototype.loadFragment.call(this, e, r, i)));
                        }),
                        (l.completeAudioSwitch = function () {
                            var e = this.hls,
                                r = this.media,
                                i = this.trackId;
                            r && (this.log("Switching audio track : flushing all audio"), t.prototype.flushMainBuffer.call(this, 0, Number.POSITIVE_INFINITY, "audio")),
                                (this.audioSwitch = !1),
                                e.trigger(a.a.AUDIO_TRACK_SWITCHED, { id: i });
                        }),
                        i
                    );
                })(Yt);
                function ce(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function fe(t, e) {
                    return (fe =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var ge = (function (t) {
                    var e, r;
                    function i(e) {
                        var r;
                        return ((r = t.call(this, e, "[audio-track-controller]") || this).tracks = []), (r.groupId = null), (r.tracksInGroup = []), (r.trackId = -1), (r.trackName = ""), (r.selectDefaultTrack = !0), r.registerListeners(), r;
                    }
                    (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), fe(e, r);
                    var n,
                        o,
                        l,
                        u = i.prototype;
                    return (
                        (u.registerListeners = function () {
                            var t = this.hls;
                            t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                t.on(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                t.on(a.a.LEVEL_SWITCHING, this.onLevelSwitching, this),
                                t.on(a.a.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this),
                                t.on(a.a.ERROR, this.onError, this);
                        }),
                        (u.unregisterListeners = function () {
                            var t = this.hls;
                            t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                t.off(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                t.off(a.a.LEVEL_SWITCHING, this.onLevelSwitching, this),
                                t.off(a.a.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this),
                                t.off(a.a.ERROR, this.onError, this);
                        }),
                        (u.destroy = function () {
                            this.unregisterListeners(), (this.tracks.length = 0), (this.tracksInGroup.length = 0), t.prototype.destroy.call(this);
                        }),
                        (u.onManifestLoading = function () {
                            (this.tracks = []), (this.groupId = null), (this.tracksInGroup = []), (this.trackId = -1), (this.trackName = ""), (this.selectDefaultTrack = !0);
                        }),
                        (u.onManifestParsed = function (t, e) {
                            this.tracks = e.audioTracks || [];
                        }),
                        (u.onAudioTrackLoaded = function (t, e) {
                            var r = e.id,
                                i = e.details,
                                n = this.tracksInGroup[r];
                            if (n) {
                                var a = n.details;
                                (n.details = e.details), this.log("audioTrack " + r + " loaded [" + i.startSN + "-" + i.endSN + "]"), r === this.trackId && ((this.retryCount = 0), this.playlistLoaded(r, e, a));
                            } else this.warn("Invalid audio track id " + r);
                        }),
                        (u.onLevelLoading = function (t, e) {
                            this.switchLevel(e.level);
                        }),
                        (u.onLevelSwitching = function (t, e) {
                            this.switchLevel(e.level);
                        }),
                        (u.switchLevel = function (t) {
                            var e = this.hls.levels[t];
                            if (null != e && e.audioGroupIds) {
                                var r = e.audioGroupIds[e.urlId];
                                if (this.groupId !== r) {
                                    this.groupId = r;
                                    var i = this.tracks.filter(function (t) {
                                        return !r || t.groupId === r;
                                    });
                                    this.selectDefaultTrack &&
                                        !i.some(function (t) {
                                            return t.default;
                                        }) &&
                                        (this.selectDefaultTrack = !1),
                                        (this.tracksInGroup = i);
                                    var n = { audioTracks: i };
                                    this.log("Updating audio tracks, " + i.length + ' track(s) found in "' + r + '" group-id'), this.hls.trigger(a.a.AUDIO_TRACKS_UPDATED, n), this.selectInitialTrack();
                                }
                            }
                        }),
                        (u.onError = function (e, r) {
                            t.prototype.onError.call(this, e, r), !r.fatal && r.context && r.context.type === k.a.AUDIO_TRACK && r.context.id === this.trackId && r.context.groupId === this.groupId && this.retryLoadingOrFail(r);
                        }),
                        (u.setAudioTrack = function (t) {
                            var e = this.tracksInGroup;
                            if (t < 0 || t >= e.length) this.warn("Invalid id passed to audio-track controller");
                            else {
                                this.clearTimer();
                                var r = e[this.trackId];
                                this.log("Now switching to audio-track index " + t);
                                var i = e[t],
                                    n = i.id,
                                    s = i.groupId,
                                    o = void 0 === s ? "" : s,
                                    l = i.name,
                                    u = i.type,
                                    h = i.url;
                                if (((this.trackId = t), (this.trackName = l), (this.selectDefaultTrack = !1), this.hls.trigger(a.a.AUDIO_TRACK_SWITCHING, { id: n, groupId: o, name: l, type: u, url: h }), !i.details || i.details.live)) {
                                    var d = this.switchParams(i.url, null == r ? void 0 : r.details);
                                    this.loadPlaylist(d);
                                }
                            }
                        }),
                        (u.selectInitialTrack = function () {
                            this.tracksInGroup;
                            var t = this.trackName,
                                e = this.findTrackId(t) || this.findTrackId();
                            -1 !== e
                                ? this.setAudioTrack(e)
                                : (this.warn("No track found for running audio group-ID: " + this.groupId), this.hls.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.AUDIO_TRACK_LOAD_ERROR, fatal: !0 }));
                        }),
                        (u.findTrackId = function (t) {
                            for (var e = this.tracksInGroup, r = 0; r < e.length; r++) {
                                var i = e[r];
                                if ((!this.selectDefaultTrack || i.default) && (!t || t === i.name)) return i.id;
                            }
                            return -1;
                        }),
                        (u.loadPlaylist = function (t) {
                            var e = this.tracksInGroup[this.trackId];
                            if (this.shouldLoadTrack(e)) {
                                var r = e.id,
                                    i = e.groupId,
                                    n = e.url;
                                if (t)
                                    try {
                                        n = t.addDirectives(n);
                                    } catch (t) {
                                        this.warn("Could not construct new URL with HLS Delivery Directives: " + t);
                                    }
                                this.log("loading audio-track playlist for id: " + r), this.clearTimer(), this.hls.trigger(a.a.AUDIO_TRACK_LOADING, { url: n, id: r, groupId: i, deliveryDirectives: t || null });
                            }
                        }),
                        (n = i),
                        (o = [
                            {
                                key: "audioTracks",
                                get: function () {
                                    return this.tracksInGroup;
                                },
                            },
                            {
                                key: "audioTrack",
                                get: function () {
                                    return this.trackId;
                                },
                                set: function (t) {
                                    (this.selectDefaultTrack = !1), this.setAudioTrack(t);
                                },
                            },
                        ]) && ce(n.prototype, o),
                        l && ce(n, l),
                        i
                    );
                })(Q);
                function ve(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function pe(t, e) {
                    return (pe =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                var me = (function (t) {
                    var e, r;
                    function i(e, r) {
                        var i;
                        return ((i = t.call(this, e, r, "[subtitle-stream-controller]") || this).levels = []), (i.currentTrackId = -1), (i.tracksBuffered = []), (i.mainDetails = null), i._registerListeners(), i;
                    }
                    (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), pe(e, r);
                    var n,
                        s,
                        l,
                        u = i.prototype;
                    return (
                        (u.onHandlerDestroying = function () {
                            this._unregisterListeners(), (this.mainDetails = null);
                        }),
                        (u._registerListeners = function () {
                            var t = this.hls;
                            t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.on(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                t.on(a.a.ERROR, this.onError, this),
                                t.on(a.a.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this),
                                t.on(a.a.SUBTITLE_TRACK_SWITCH, this.onSubtitleTrackSwitch, this),
                                t.on(a.a.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this),
                                t.on(a.a.SUBTITLE_FRAG_PROCESSED, this.onSubtitleFragProcessed, this),
                                t.on(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this);
                        }),
                        (u._unregisterListeners = function () {
                            var t = this.hls;
                            t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.off(a.a.LEVEL_LOADED, this.onLevelLoaded, this),
                                t.off(a.a.ERROR, this.onError, this),
                                t.off(a.a.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this),
                                t.off(a.a.SUBTITLE_TRACK_SWITCH, this.onSubtitleTrackSwitch, this),
                                t.off(a.a.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this),
                                t.off(a.a.SUBTITLE_FRAG_PROCESSED, this.onSubtitleFragProcessed, this),
                                t.off(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this);
                        }),
                        (u.startLoad = function () {
                            this.stopLoad(), (this.state = Pt), this.setInterval(500), this.tick();
                        }),
                        (u.onManifestLoading = function () {
                            (this.mainDetails = null), this.fragmentTracker.removeAllFragments();
                        }),
                        (u.onLevelLoaded = function (t, e) {
                            this.mainDetails = e.details;
                        }),
                        (u.onSubtitleFragProcessed = function (t, e) {
                            var r = e.frag,
                                i = e.success;
                            if (((this.fragPrevious = r), (this.state = Pt), i)) {
                                var n = this.tracksBuffered[this.currentTrackId];
                                if (n) {
                                    for (var a, s = r.start, o = 0; o < n.length; o++)
                                        if (s >= n[o].start && s <= n[o].end) {
                                            a = n[o];
                                            break;
                                        }
                                    var l = r.start + r.duration;
                                    a ? (a.end = l) : ((a = { start: s, end: l }), n.push(a)), this.fragmentTracker.fragBuffered(r);
                                }
                            }
                        }),
                        (u.onBufferFlushing = function (t, e) {
                            var r = e.startOffset,
                                i = e.endOffset;
                            if (0 === r && i !== Number.POSITIVE_INFINITY) {
                                var n = this.currentTrackId,
                                    a = this.levels;
                                if (!a.length || !a[n] || !a[n].details) return;
                                var s = i - a[n].details.targetduration;
                                if (s <= 0) return;
                                (e.endOffsetSubtitles = Math.max(0, s)),
                                    this.tracksBuffered.forEach(function (t) {
                                        for (var e = 0; e < t.length; )
                                            if (t[e].end <= s) t.shift();
                                            else {
                                                if (!(t[e].start < s)) break;
                                                (t[e].start = s), e++;
                                            }
                                    }),
                                    this.fragmentTracker.removeFragmentsInRange(r, s, k.b.SUBTITLE);
                            }
                        }),
                        (u.onError = function (t, e) {
                            var r,
                                i = e.frag;
                            i && i.type === k.b.SUBTITLE && (null !== (r = this.fragCurrent) && void 0 !== r && r.loader && this.fragCurrent.loader.abort(), (this.state = Pt));
                        }),
                        (u.onSubtitleTracksUpdated = function (t, e) {
                            var r = this,
                                i = e.subtitleTracks;
                            (this.tracksBuffered = []),
                                (this.levels = i.map(function (t) {
                                    return new j(t);
                                })),
                                this.fragmentTracker.removeAllFragments(),
                                (this.fragPrevious = null),
                                this.levels.forEach(function (t) {
                                    r.tracksBuffered[t.id] = [];
                                }),
                                (this.mediaBuffer = null);
                        }),
                        (u.onSubtitleTrackSwitch = function (t, e) {
                            if (((this.currentTrackId = e.id), this.levels.length && -1 !== this.currentTrackId)) {
                                var r = this.levels[this.currentTrackId];
                                null != r && r.details ? ((this.mediaBuffer = this.mediaBufferTimeRanges), this.setInterval(500)) : (this.mediaBuffer = null);
                            } else this.clearInterval();
                        }),
                        (u.onSubtitleTrackLoaded = function (t, e) {
                            var r,
                                i = e.details,
                                n = e.id,
                                a = this.currentTrackId,
                                s = this.levels;
                            if (s.length) {
                                var o = s[a];
                                if (!(n >= s.length || n !== a) && o) {
                                    if (((this.mediaBuffer = this.mediaBufferTimeRanges), i.live || (null !== (r = o.details) && void 0 !== r && r.live))) {
                                        var l = this.mainDetails;
                                        if (i.deltaUpdateFailed || !l) return;
                                        var u = l.fragments[0];
                                        if (o.details) 0 === this.alignPlaylists(i, o.details) && u && z(i, u.start);
                                        else i.hasProgramDateTime && l.hasProgramDateTime ? gt(i, l) : u && z(i, u.start);
                                    }
                                    if (((o.details = i), (this.levelLastLoaded = n), this.tick(), i.live && !this.fragCurrent && this.media && this.state === Pt))
                                        mt(null, i.fragments, this.media.currentTime, 0) || (this.warn("Subtitle playlist not aligned with playback"), (o.details = void 0));
                                }
                            }
                        }),
                        (u._handleFragmentLoadComplete = function (t) {
                            var e = t.frag,
                                r = t.payload,
                                i = e.decryptdata,
                                n = this.hls;
                            if (!this.fragContextChanged(e) && r && r.byteLength > 0 && i && i.key && i.iv && "AES-128" === i.method) {
                                var s = performance.now();
                                this.decrypter.webCryptoDecrypt(new Uint8Array(r), i.key.buffer, i.iv.buffer).then(function (t) {
                                    var r = performance.now();
                                    n.trigger(a.a.FRAG_DECRYPTED, { frag: e, payload: t, stats: { tstart: s, tdecrypt: r } });
                                });
                            }
                        }),
                        (u.doTick = function () {
                            if (this.media) {
                                if (this.state === Pt) {
                                    var t,
                                        e = this.currentTrackId,
                                        r = this.levels;
                                    if (!r.length || !r[e] || !r[e].details) return;
                                    var i = r[e].details,
                                        n = i.targetduration,
                                        s = this.config,
                                        l = this.media,
                                        u = lt.bufferedInfo(this.mediaBufferTimeRanges, l.currentTime - n, s.maxBufferHole),
                                        h = u.end;
                                    if (u.len > this.getMaxBufferLength() + n) return;
                                    var d,
                                        c = i.fragments,
                                        f = c.length,
                                        g = i.edge,
                                        v = this.fragPrevious;
                                    if (h < g) {
                                        var p = s.maxFragLookUpTolerance;
                                        v && i.hasProgramDateTime && (d = pt(c, v.endProgramDateTime, p)), d || (!(d = mt(v, c, h, p)) && v && v.start < c[0].start && (d = c[0]));
                                    } else d = c[f - 1];
                                    null !== (t = d) && void 0 !== t && t.encrypted
                                        ? (o.b.log("Loading key for " + d.sn), (this.state = Mt), this.hls.trigger(a.a.KEY_LOADING, { frag: d }))
                                        : d && this.fragmentTracker.getState(d) === tt.NOT_LOADED && this.loadFragment(d, i, h);
                                }
                            } else this.state = Pt;
                        }),
                        (u.loadFragment = function (e, r, i) {
                            (this.fragCurrent = e), t.prototype.loadFragment.call(this, e, r, i);
                        }),
                        (n = i),
                        (s = [
                            {
                                key: "mediaBufferTimeRanges",
                                get: function () {
                                    return this.tracksBuffered[this.currentTrackId] || [];
                                },
                            },
                        ]) && ve(n.prototype, s),
                        l && ve(n, l),
                        i
                    );
                })(Yt);
                function ye(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function Te(t, e) {
                    return (Te =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                function be(t) {
                    for (var e = [], r = 0; r < t.length; r++) {
                        var i = t[r];
                        "subtitles" === i.kind && i.label && e.push(t[r]);
                    }
                    return e;
                }
                var Ee,
                    Se = (function (t) {
                        var e, r;
                        function i(e) {
                            var r;
                            return (
                                ((r = t.call(this, e, "[subtitle-track-controller]") || this).media = null),
                                (r.tracks = []),
                                (r.groupId = null),
                                (r.tracksInGroup = []),
                                (r.trackId = -1),
                                (r.selectDefaultTrack = !0),
                                (r.queuedDefaultTrack = -1),
                                (r.trackChangeListener = function () {
                                    return r.onTextTracksChanged();
                                }),
                                (r.asyncPollTrackChange = function () {
                                    return r.pollTrackChange(0);
                                }),
                                (r.useTextTrackPolling = !1),
                                (r.subtitlePollingInterval = -1),
                                (r.subtitleDisplay = !0),
                                r.registerListeners(),
                                r
                            );
                        }
                        (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), Te(e, r);
                        var n,
                            s,
                            o,
                            l = i.prototype;
                        return (
                            (l.destroy = function () {
                                this.unregisterListeners(), (this.tracks.length = 0), (this.tracksInGroup.length = 0), (this.trackChangeListener = this.asyncPollTrackChange = null), t.prototype.destroy.call(this);
                            }),
                            (l.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.on(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                    t.on(a.a.LEVEL_SWITCHING, this.onLevelSwitching, this),
                                    t.on(a.a.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this),
                                    t.on(a.a.ERROR, this.onError, this);
                            }),
                            (l.unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this),
                                    t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                    t.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.off(a.a.LEVEL_LOADING, this.onLevelLoading, this),
                                    t.off(a.a.LEVEL_SWITCHING, this.onLevelSwitching, this),
                                    t.off(a.a.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this),
                                    t.off(a.a.ERROR, this.onError, this);
                            }),
                            (l.onMediaAttached = function (t, e) {
                                (this.media = e.media),
                                    this.media &&
                                        (this.queuedDefaultTrack > -1 && ((this.subtitleTrack = this.queuedDefaultTrack), (this.queuedDefaultTrack = -1)),
                                        (this.useTextTrackPolling = !(this.media.textTracks && "onchange" in this.media.textTracks)),
                                        this.useTextTrackPolling ? this.pollTrackChange(500) : this.media.textTracks.addEventListener("change", this.asyncPollTrackChange));
                            }),
                            (l.pollTrackChange = function (t) {
                                self.clearInterval(this.subtitlePollingInterval), (this.subtitlePollingInterval = self.setInterval(this.trackChangeListener, t));
                            }),
                            (l.onMediaDetaching = function () {
                                this.media &&
                                    (self.clearInterval(this.subtitlePollingInterval),
                                    this.useTextTrackPolling || this.media.textTracks.removeEventListener("change", this.asyncPollTrackChange),
                                    this.trackId > -1 && (this.queuedDefaultTrack = this.trackId),
                                    be(this.media.textTracks).forEach(function (t) {
                                        x(t);
                                    }),
                                    (this.subtitleTrack = -1),
                                    (this.media = null));
                            }),
                            (l.onManifestLoading = function () {
                                (this.tracks = []), (this.groupId = null), (this.tracksInGroup = []), (this.trackId = -1), (this.selectDefaultTrack = !0);
                            }),
                            (l.onManifestParsed = function (t, e) {
                                this.tracks = e.subtitleTracks;
                            }),
                            (l.onSubtitleTrackLoaded = function (t, e) {
                                var r = e.id,
                                    i = e.details,
                                    n = this.trackId,
                                    a = this.tracksInGroup[n];
                                if (a) {
                                    var s = a.details;
                                    (a.details = e.details), this.log("subtitle track " + r + " loaded [" + i.startSN + "-" + i.endSN + "]"), r === this.trackId && ((this.retryCount = 0), this.playlistLoaded(r, e, s));
                                } else this.warn("Invalid subtitle track id " + r);
                            }),
                            (l.onLevelLoading = function (t, e) {
                                this.switchLevel(e.level);
                            }),
                            (l.onLevelSwitching = function (t, e) {
                                this.switchLevel(e.level);
                            }),
                            (l.switchLevel = function (t) {
                                var e = this.hls.levels[t];
                                if (null != e && e.textGroupIds) {
                                    var r = e.textGroupIds[e.urlId];
                                    if (this.groupId !== r) {
                                        var i = this.tracksInGroup ? this.tracksInGroup[this.trackId] : void 0,
                                            n = this.tracks.filter(function (t) {
                                                return !r || t.groupId === r;
                                            });
                                        this.tracksInGroup = n;
                                        var s = this.findTrackId(null == i ? void 0 : i.name) || this.findTrackId();
                                        this.groupId = r;
                                        var o = { subtitleTracks: n };
                                        this.log("Updating subtitle tracks, " + n.length + ' track(s) found in "' + r + '" group-id'), this.hls.trigger(a.a.SUBTITLE_TRACKS_UPDATED, o), -1 !== s && this.setSubtitleTrack(s, i);
                                    }
                                }
                            }),
                            (l.findTrackId = function (t) {
                                for (var e = this.tracksInGroup, r = 0; r < e.length; r++) {
                                    var i = e[r];
                                    if ((!this.selectDefaultTrack || i.default) && (!t || t === i.name)) return i.id;
                                }
                                return -1;
                            }),
                            (l.onError = function (e, r) {
                                t.prototype.onError.call(this, e, r), !r.fatal && r.context && r.context.type === k.a.SUBTITLE_TRACK && r.context.id === this.trackId && r.context.groupId === this.groupId && this.retryLoadingOrFail(r);
                            }),
                            (l.loadPlaylist = function (t) {
                                var e = this.tracksInGroup[this.trackId];
                                if (this.shouldLoadTrack(e)) {
                                    var r = e.id,
                                        i = e.groupId,
                                        n = e.url;
                                    if (t)
                                        try {
                                            n = t.addDirectives(n);
                                        } catch (t) {
                                            this.warn("Could not construct new URL with HLS Delivery Directives: " + t);
                                        }
                                    this.log("Loading subtitle playlist for id " + r), this.hls.trigger(a.a.SUBTITLE_TRACK_LOADING, { url: n, id: r, groupId: i, deliveryDirectives: t || null });
                                }
                            }),
                            (l.toggleTrackModes = function (t) {
                                var e = this,
                                    r = this.media,
                                    i = this.subtitleDisplay,
                                    n = this.trackId;
                                if (r) {
                                    var a = be(r.textTracks),
                                        s = a.filter(function (t) {
                                            return t.groupId === e.groupId;
                                        });
                                    if (-1 === t)
                                        [].slice.call(a).forEach(function (t) {
                                            t.mode = "disabled";
                                        });
                                    else {
                                        var o = s[n];
                                        o && (o.mode = "disabled");
                                    }
                                    var l = s[t];
                                    l && (l.mode = i ? "showing" : "hidden");
                                }
                            }),
                            (l.setSubtitleTrack = function (t, e) {
                                var r,
                                    i = this.tracksInGroup;
                                if (this.media) {
                                    if ((this.trackId !== t && this.toggleTrackModes(t), !((this.trackId === t && (-1 === t || (null !== (r = i[t]) && void 0 !== r && r.details))) || t < -1 || t >= i.length))) {
                                        this.clearTimer();
                                        var n = i[t];
                                        if ((this.log("Switching to subtitle track " + t), (this.trackId = t), n)) {
                                            var s = n.id,
                                                o = n.groupId,
                                                l = void 0 === o ? "" : o,
                                                u = n.name,
                                                h = n.type,
                                                d = n.url;
                                            this.hls.trigger(a.a.SUBTITLE_TRACK_SWITCH, { id: s, groupId: l, name: u, type: h, url: d });
                                            var c = this.switchParams(n.url, null == e ? void 0 : e.details);
                                            this.loadPlaylist(c);
                                        } else this.hls.trigger(a.a.SUBTITLE_TRACK_SWITCH, { id: t });
                                    }
                                } else this.queuedDefaultTrack = t;
                            }),
                            (l.onTextTracksChanged = function () {
                                if ((this.useTextTrackPolling || self.clearInterval(this.subtitlePollingInterval), this.media && this.hls.config.renderTextTracksNatively)) {
                                    for (var t = -1, e = be(this.media.textTracks), r = 0; r < e.length; r++)
                                        if ("hidden" === e[r].mode) t = r;
                                        else if ("showing" === e[r].mode) {
                                            t = r;
                                            break;
                                        }
                                    this.subtitleTrack !== t && (this.subtitleTrack = t);
                                }
                            }),
                            (n = i),
                            (s = [
                                {
                                    key: "subtitleTracks",
                                    get: function () {
                                        return this.tracksInGroup;
                                    },
                                },
                                {
                                    key: "subtitleTrack",
                                    get: function () {
                                        return this.trackId;
                                    },
                                    set: function (t) {
                                        this.selectDefaultTrack = !1;
                                        var e = this.tracksInGroup ? this.tracksInGroup[this.trackId] : void 0;
                                        this.setSubtitleTrack(t, e);
                                    },
                                },
                            ]) && ye(n.prototype, s),
                            o && ye(n, o),
                            i
                        );
                    })(Q),
                    Le = (function () {
                        function t(t) {
                            (this.buffers = void 0), (this.queues = { video: [], audio: [], audiovideo: [] }), (this.buffers = t);
                        }
                        var e = t.prototype;
                        return (
                            (e.append = function (t, e) {
                                var r = this.queues[e];
                                r.push(t), 1 === r.length && this.buffers[e] && this.executeNext(e);
                            }),
                            (e.insertAbort = function (t, e) {
                                this.queues[e].unshift(t), this.executeNext(e);
                            }),
                            (e.appendBlocker = function (t) {
                                var e,
                                    r = new Promise(function (t) {
                                        e = t;
                                    }),
                                    i = { execute: e, onStart: function () {}, onComplete: function () {}, onError: function () {} };
                                return this.append(i, t), r;
                            }),
                            (e.executeNext = function (t) {
                                var e = this.buffers,
                                    r = this.queues,
                                    i = e[t],
                                    n = r[t];
                                if (n.length) {
                                    var a = n[0];
                                    try {
                                        a.execute();
                                    } catch (e) {
                                        o.b.warn("[buffer-operation-queue]: Unhandled exception executing the current operation"), a.onError(e), (i && i.updating) || (n.shift(), this.executeNext(t));
                                    }
                                }
                            }),
                            (e.shiftAndExecuteNext = function (t) {
                                this.queues[t].shift(), this.executeNext(t);
                            }),
                            (e.current = function (t) {
                                return this.queues[t][0];
                            }),
                            t
                        );
                    })(),
                    Ae = qt(),
                    Re = /([ha]vc.)(?:\.[^.,]+)+/,
                    De = (function () {
                        function t(t) {
                            var e = this;
                            (this.details = null),
                                (this._objectUrl = null),
                                (this.operationQueue = void 0),
                                (this.listeners = void 0),
                                (this.hls = void 0),
                                (this.bufferCodecEventsExpected = 0),
                                (this._bufferCodecEventsTotal = 0),
                                (this.media = null),
                                (this.mediaSource = null),
                                (this.appendError = 0),
                                (this.tracks = {}),
                                (this.pendingTracks = {}),
                                (this.sourceBuffer = void 0),
                                (this._onMediaSourceOpen = function () {
                                    var t = e.hls,
                                        r = e.media,
                                        i = e.mediaSource;
                                    o.b.log("[buffer-controller]: Media source opened"),
                                        r && (e.updateMediaElementDuration(), t.trigger(a.a.MEDIA_ATTACHED, { media: r })),
                                        i && i.removeEventListener("sourceopen", e._onMediaSourceOpen),
                                        e.checkPendingTracks();
                                }),
                                (this._onMediaSourceClose = function () {
                                    o.b.log("[buffer-controller]: Media source closed");
                                }),
                                (this._onMediaSourceEnded = function () {
                                    o.b.log("[buffer-controller]: Media source ended");
                                }),
                                (this.hls = t),
                                this._initSourceBuffer(),
                                this.registerListeners();
                        }
                        var e = t.prototype;
                        return (
                            (e.hasSourceTypes = function () {
                                return this.getSourceBufferTypes().length > 0 || Object.keys(this.pendingTracks).length > 0;
                            }),
                            (e.destroy = function () {
                                this.unregisterListeners(), (this.details = null);
                            }),
                            (e.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                                    t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.on(a.a.BUFFER_RESET, this.onBufferReset, this),
                                    t.on(a.a.BUFFER_APPENDING, this.onBufferAppending, this),
                                    t.on(a.a.BUFFER_CODECS, this.onBufferCodecs, this),
                                    t.on(a.a.BUFFER_EOS, this.onBufferEos, this),
                                    t.on(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this),
                                    t.on(a.a.LEVEL_UPDATED, this.onLevelUpdated, this),
                                    t.on(a.a.FRAG_PARSED, this.onFragParsed, this),
                                    t.on(a.a.FRAG_CHANGED, this.onFragChanged, this);
                            }),
                            (e.unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                                    t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                    t.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.off(a.a.BUFFER_RESET, this.onBufferReset, this),
                                    t.off(a.a.BUFFER_APPENDING, this.onBufferAppending, this),
                                    t.off(a.a.BUFFER_CODECS, this.onBufferCodecs, this),
                                    t.off(a.a.BUFFER_EOS, this.onBufferEos, this),
                                    t.off(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this),
                                    t.off(a.a.LEVEL_UPDATED, this.onLevelUpdated, this),
                                    t.off(a.a.FRAG_PARSED, this.onFragParsed, this),
                                    t.off(a.a.FRAG_CHANGED, this.onFragChanged, this);
                            }),
                            (e._initSourceBuffer = function () {
                                (this.sourceBuffer = {}), (this.operationQueue = new Le(this.sourceBuffer)), (this.listeners = { audio: [], video: [], audiovideo: [] });
                            }),
                            (e.onManifestParsed = function (t, e) {
                                var r = 2;
                                ((e.audio && !e.video) || !e.altAudio) && (r = 1),
                                    (this.bufferCodecEventsExpected = this._bufferCodecEventsTotal = r),
                                    (this.details = null),
                                    o.b.log(this.bufferCodecEventsExpected + " bufferCodec event(s) expected");
                            }),
                            (e.onMediaAttaching = function (t, e) {
                                var r = (this.media = e.media);
                                if (r && Ae) {
                                    var i = (this.mediaSource = new Ae());
                                    i.addEventListener("sourceopen", this._onMediaSourceOpen),
                                        i.addEventListener("sourceended", this._onMediaSourceEnded),
                                        i.addEventListener("sourceclose", this._onMediaSourceClose),
                                        (r.src = self.URL.createObjectURL(i)),
                                        (this._objectUrl = r.src);
                                }
                            }),
                            (e.onMediaDetaching = function () {
                                var t = this.media,
                                    e = this.mediaSource,
                                    r = this._objectUrl;
                                if (e) {
                                    if ((o.b.log("[buffer-controller]: media source detaching"), "open" === e.readyState))
                                        try {
                                            e.endOfStream();
                                        } catch (t) {
                                            o.b.warn("[buffer-controller]: onMediaDetaching: " + t.message + " while calling endOfStream");
                                        }
                                    this.onBufferReset(),
                                        e.removeEventListener("sourceopen", this._onMediaSourceOpen),
                                        e.removeEventListener("sourceended", this._onMediaSourceEnded),
                                        e.removeEventListener("sourceclose", this._onMediaSourceClose),
                                        t && (r && self.URL.revokeObjectURL(r), t.src === r ? (t.removeAttribute("src"), t.load()) : o.b.warn("[buffer-controller]: media.src was changed by a third party - skip cleanup")),
                                        (this.mediaSource = null),
                                        (this.media = null),
                                        (this._objectUrl = null),
                                        (this.bufferCodecEventsExpected = this._bufferCodecEventsTotal),
                                        (this.pendingTracks = {}),
                                        (this.tracks = {});
                                }
                                this.hls.trigger(a.a.MEDIA_DETACHED, void 0);
                            }),
                            (e.onBufferReset = function () {
                                var t = this;
                                this.getSourceBufferTypes().forEach(function (e) {
                                    var r = t.sourceBuffer[e];
                                    try {
                                        r && (t.removeBufferListeners(e), t.mediaSource && t.mediaSource.removeSourceBuffer(r), (t.sourceBuffer[e] = void 0));
                                    } catch (t) {
                                        o.b.warn("[buffer-controller]: Failed to reset the " + e + " buffer", t);
                                    }
                                }),
                                    this._initSourceBuffer();
                            }),
                            (e.onBufferCodecs = function (t, e) {
                                var r = this,
                                    i = this.getSourceBufferTypes().length;
                                Object.keys(e).forEach(function (t) {
                                    if (i) {
                                        var n = r.tracks[t];
                                        if (n && "function" == typeof n.buffer.changeType) {
                                            var a = e[t],
                                                s = a.codec,
                                                o = a.levelCodec,
                                                l = a.container;
                                            if ((n.levelCodec || n.codec).replace(Re, "$1") !== (o || s).replace(Re, "$1")) {
                                                var u = l + ";codecs=" + (o || s);
                                                r.appendChangeType(t, u);
                                            }
                                        }
                                    } else r.pendingTracks[t] = e[t];
                                }),
                                    i || ((this.bufferCodecEventsExpected = Math.max(this.bufferCodecEventsExpected - 1, 0)), this.mediaSource && "open" === this.mediaSource.readyState && this.checkPendingTracks());
                            }),
                            (e.appendChangeType = function (t, e) {
                                var r = this,
                                    i = this.operationQueue,
                                    n = {
                                        execute: function () {
                                            var n = r.sourceBuffer[t];
                                            n && (o.b.log("[buffer-controller]: changing " + t + " sourceBuffer type to " + e), n.changeType(e)), i.shiftAndExecuteNext(t);
                                        },
                                        onStart: function () {},
                                        onComplete: function () {},
                                        onError: function (e) {
                                            o.b.warn("[buffer-controller]: Failed to change " + t + " SourceBuffer type", e);
                                        },
                                    };
                                i.append(n, t);
                            }),
                            (e.onBufferAppending = function (t, e) {
                                var r = this,
                                    i = this.hls,
                                    n = this.operationQueue,
                                    l = this.tracks,
                                    u = e.data,
                                    h = e.type,
                                    d = e.frag,
                                    c = e.part,
                                    f = e.chunkMeta,
                                    g = f.buffering[h],
                                    v = self.performance.now();
                                g.start = v;
                                var p = d.stats.buffering,
                                    m = c ? c.stats.buffering : null;
                                0 === p.start && (p.start = v), m && 0 === m.start && (m.start = v);
                                var y = l.audio,
                                    T = "audio" === h && 1 === f.id && "audio/mpeg" === (null == y ? void 0 : y.container),
                                    b = {
                                        execute: function () {
                                            if (((g.executeStart = self.performance.now()), T)) {
                                                var t = r.sourceBuffer[h];
                                                if (t) {
                                                    var e = d.start - t.timestampOffset;
                                                    Math.abs(e) >= 0.1 && (o.b.log("[buffer-controller]: Updating audio SourceBuffer timestampOffset to " + d.start + " (delta: " + e + ") sn: " + d.sn + ")"), (t.timestampOffset = d.start));
                                                }
                                            }
                                            r.appendExecutor(u, h);
                                        },
                                        onStart: function () {},
                                        onComplete: function () {
                                            var t = self.performance.now();
                                            (g.executeEnd = g.end = t), 0 === p.first && (p.first = t), m && 0 === m.first && (m.first = t);
                                            var e = r.sourceBuffer,
                                                i = {};
                                            for (var n in e) i[n] = lt.getBuffered(e[n]);
                                            (r.appendError = 0), r.hls.trigger(a.a.BUFFER_APPENDED, { type: h, frag: d, part: c, chunkMeta: f, parent: d.type, timeRanges: i });
                                        },
                                        onError: function (t) {
                                            o.b.error("[buffer-controller]: Error encountered while trying to append to the " + h + " SourceBuffer", t);
                                            var e = { type: s.b.MEDIA_ERROR, parent: d.type, details: s.a.BUFFER_APPEND_ERROR, err: t, fatal: !1 };
                                            t.code === DOMException.QUOTA_EXCEEDED_ERR
                                                ? (e.details = s.a.BUFFER_FULL_ERROR)
                                                : (r.appendError++,
                                                  (e.details = s.a.BUFFER_APPEND_ERROR),
                                                  r.appendError > i.config.appendErrorMaxRetry && (o.b.error("[buffer-controller]: Failed " + i.config.appendErrorMaxRetry + " times to append segment in sourceBuffer"), (e.fatal = !0))),
                                                i.trigger(a.a.ERROR, e);
                                        },
                                    };
                                n.append(b, h);
                            }),
                            (e.onBufferFlushing = function (t, e) {
                                var r = this,
                                    i = this.operationQueue,
                                    n = function (t) {
                                        return {
                                            execute: r.removeExecutor.bind(r, t, e.startOffset, e.endOffset),
                                            onStart: function () {},
                                            onComplete: function () {
                                                r.hls.trigger(a.a.BUFFER_FLUSHED, { type: t });
                                            },
                                            onError: function (e) {
                                                o.b.warn("[buffer-controller]: Failed to remove from " + t + " SourceBuffer", e);
                                            },
                                        };
                                    };
                                e.type
                                    ? i.append(n(e.type), e.type)
                                    : this.getSourceBufferTypes().forEach(function (t) {
                                          i.append(n(t), t);
                                      });
                            }),
                            (e.onFragParsed = function (t, e) {
                                var r = this,
                                    i = e.frag,
                                    n = e.part,
                                    s = [],
                                    l = n ? n.elementaryStreams : i.elementaryStreams;
                                l[u.a.AUDIOVIDEO] ? s.push("audiovideo") : (l[u.a.AUDIO] && s.push("audio"), l[u.a.VIDEO] && s.push("video"));
                                0 === s.length && o.b.warn("Fragments must have at least one ElementaryStreamType set. type: " + i.type + " level: " + i.level + " sn: " + i.sn),
                                    this.blockBuffers(function () {
                                        var t = self.performance.now();
                                        (i.stats.buffering.end = t), n && (n.stats.buffering.end = t);
                                        var e = n ? n.stats : i.stats;
                                        r.hls.trigger(a.a.FRAG_BUFFERED, { frag: i, part: n, stats: e, id: i.type });
                                    }, s);
                            }),
                            (e.onFragChanged = function (t, e) {
                                this.flushBackBuffer();
                            }),
                            (e.onBufferEos = function (t, e) {
                                var r = this;
                                this.getSourceBufferTypes().reduce(function (t, i) {
                                    var n = r.sourceBuffer[i];
                                    return (e.type && e.type !== i) || (n && !n.ended && ((n.ended = !0), o.b.log("[buffer-controller]: " + i + " sourceBuffer now EOS"))), t && !(n && !n.ended);
                                }, !0) &&
                                    this.blockBuffers(function () {
                                        var t = r.mediaSource;
                                        t && "open" === t.readyState && t.endOfStream();
                                    });
                            }),
                            (e.onLevelUpdated = function (t, e) {
                                var r = e.details;
                                r.fragments.length && ((this.details = r), this.getSourceBufferTypes().length ? this.blockBuffers(this.updateMediaElementDuration.bind(this)) : this.updateMediaElementDuration());
                            }),
                            (e.flushBackBuffer = function () {
                                var t = this.hls,
                                    e = this.details,
                                    r = this.media,
                                    i = this.sourceBuffer;
                                if (r && null !== e) {
                                    var s = this.getSourceBufferTypes();
                                    if (s.length) {
                                        var o = e.live && null !== t.config.liveBackBufferLength ? t.config.liveBackBufferLength : t.config.backBufferLength;
                                        if (Object(n.a)(o) && !(o < 0)) {
                                            var l = r.currentTime,
                                                u = e.levelTargetDuration,
                                                h = Math.max(o, u),
                                                d = Math.floor(l / u) * u - h;
                                            s.forEach(function (r) {
                                                var n = i[r];
                                                if (n) {
                                                    var s = lt.getBuffered(n);
                                                    s.length > 0 &&
                                                        d > s.start(0) &&
                                                        (t.trigger(a.a.BACK_BUFFER_REACHED, { bufferEnd: d }),
                                                        e.live && t.trigger(a.a.LIVE_BACK_BUFFER_REACHED, { bufferEnd: d }),
                                                        t.trigger(a.a.BUFFER_FLUSHING, { startOffset: 0, endOffset: d, type: r }));
                                                }
                                            });
                                        }
                                    }
                                }
                            }),
                            (e.updateMediaElementDuration = function () {
                                if (this.details && this.media && this.mediaSource && "open" === this.mediaSource.readyState) {
                                    var t = this.details,
                                        e = this.hls,
                                        r = this.media,
                                        i = this.mediaSource,
                                        a = t.fragments[0].start + t.totalduration,
                                        s = r.duration,
                                        l = Object(n.a)(i.duration) ? i.duration : 0;
                                    t.live && e.config.liveDurationInfinity
                                        ? (o.b.log("[buffer-controller]: Media Source duration is set to Infinity"), (i.duration = 1 / 0), this.updateSeekableRange(t))
                                        : ((a > l && a > s) || !Object(n.a)(s)) && (o.b.log("[buffer-controller]: Updating Media Source duration to " + a.toFixed(3)), (i.duration = a));
                                }
                            }),
                            (e.updateSeekableRange = function (t) {
                                var e = this.mediaSource,
                                    r = t.fragments;
                                if (r.length && t.live && null != e && e.setLiveSeekableRange) {
                                    var i = Math.max(0, r[0].start),
                                        n = Math.max(i, i + t.totalduration);
                                    e.setLiveSeekableRange(i, n);
                                }
                            }),
                            (e.checkPendingTracks = function () {
                                var t = this.bufferCodecEventsExpected,
                                    e = this.operationQueue,
                                    r = this.pendingTracks,
                                    i = Object.keys(r).length;
                                if ((i && !t) || 2 === i) {
                                    this.createSourceBuffers(r), (this.pendingTracks = {});
                                    var n = this.getSourceBufferTypes();
                                    if (0 === n.length)
                                        return void this.hls.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_INCOMPATIBLE_CODECS_ERROR, fatal: !0, reason: "could not create source buffer for media codec(s)" });
                                    n.forEach(function (t) {
                                        e.executeNext(t);
                                    });
                                }
                            }),
                            (e.createSourceBuffers = function (t) {
                                var e = this.sourceBuffer,
                                    r = this.mediaSource;
                                if (!r) throw Error("createSourceBuffers called when mediaSource was null");
                                var i = 0;
                                for (var n in t)
                                    if (!e[n]) {
                                        var l = t[n];
                                        if (!l) throw Error("source buffer exists for track " + n + ", however track does not");
                                        var u = l.levelCodec || l.codec,
                                            h = l.container + ";codecs=" + u;
                                        o.b.log("[buffer-controller]: creating sourceBuffer(" + h + ")");
                                        try {
                                            var d = (e[n] = r.addSourceBuffer(h)),
                                                c = n;
                                            this.addBufferListener(c, "updatestart", this._onSBUpdateStart),
                                                this.addBufferListener(c, "updateend", this._onSBUpdateEnd),
                                                this.addBufferListener(c, "error", this._onSBUpdateError),
                                                (this.tracks[n] = { buffer: d, codec: u, container: l.container, levelCodec: l.levelCodec, id: l.id }),
                                                i++;
                                        } catch (t) {
                                            o.b.error("[buffer-controller]: error while trying to add sourceBuffer: " + t.message),
                                                this.hls.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_ADD_CODEC_ERROR, fatal: !1, error: t, mimeType: h });
                                        }
                                    }
                                i && this.hls.trigger(a.a.BUFFER_CREATED, { tracks: this.tracks });
                            }),
                            (e._onSBUpdateStart = function (t) {
                                this.operationQueue.current(t).onStart();
                            }),
                            (e._onSBUpdateEnd = function (t) {
                                var e = this.operationQueue;
                                e.current(t).onComplete(), e.shiftAndExecuteNext(t);
                            }),
                            (e._onSBUpdateError = function (t, e) {
                                o.b.error("[buffer-controller]: " + t + " SourceBuffer error", e), this.hls.trigger(a.a.ERROR, { type: s.b.MEDIA_ERROR, details: s.a.BUFFER_APPENDING_ERROR, fatal: !1 });
                                var r = this.operationQueue.current(t);
                                r && r.onError(e);
                            }),
                            (e.removeExecutor = function (t, e, r) {
                                var i = this.media,
                                    a = this.mediaSource,
                                    s = this.operationQueue,
                                    l = this.sourceBuffer[t];
                                if (!i || !a || !l) return o.b.warn("[buffer-controller]: Attempting to remove from the " + t + " SourceBuffer, but it does not exist"), void s.shiftAndExecuteNext(t);
                                var u = Object(n.a)(i.duration) ? i.duration : 1 / 0,
                                    h = Object(n.a)(a.duration) ? a.duration : 1 / 0,
                                    d = Math.max(0, e),
                                    c = Math.min(r, u, h);
                                c > d ? (o.b.log("[buffer-controller]: Removing [" + d + "," + c + "] from the " + t + " SourceBuffer"), l.remove(d, c)) : s.shiftAndExecuteNext(t);
                            }),
                            (e.appendExecutor = function (t, e) {
                                var r = this.operationQueue,
                                    i = this.sourceBuffer[e];
                                if (!i) return o.b.warn("[buffer-controller]: Attempting to append to the " + e + " SourceBuffer, but it does not exist"), void r.shiftAndExecuteNext(e);
                                (i.ended = !1), i.appendBuffer(t);
                            }),
                            (e.blockBuffers = function (t, e) {
                                var r = this;
                                if ((void 0 === e && (e = this.getSourceBufferTypes()), !e.length)) return o.b.log("[buffer-controller]: Blocking operation requested, but no SourceBuffers exist"), void Promise.resolve(t);
                                var i = this.operationQueue,
                                    n = e.map(function (t) {
                                        return i.appendBlocker(t);
                                    });
                                Promise.all(n).then(function () {
                                    t(),
                                        e.forEach(function (t) {
                                            var e = r.sourceBuffer[t];
                                            (e && e.updating) || i.shiftAndExecuteNext(t);
                                        });
                                });
                            }),
                            (e.getSourceBufferTypes = function () {
                                return Object.keys(this.sourceBuffer);
                            }),
                            (e.addBufferListener = function (t, e, r) {
                                var i = this.sourceBuffer[t];
                                if (i) {
                                    var n = r.bind(this, t);
                                    this.listeners[t].push({ event: e, listener: n }), i.addEventListener(e, n);
                                }
                            }),
                            (e.removeBufferListeners = function (t) {
                                var e = this.sourceBuffer[t];
                                e &&
                                    this.listeners[t].forEach(function (t) {
                                        e.removeEventListener(t.event, t.listener);
                                    });
                            }),
                            t
                        );
                    })(),
                    ke = {
                        42: 225,
                        92: 233,
                        94: 237,
                        95: 243,
                        96: 250,
                        123: 231,
                        124: 247,
                        125: 209,
                        126: 241,
                        127: 9608,
                        128: 174,
                        129: 176,
                        130: 189,
                        131: 191,
                        132: 8482,
                        133: 162,
                        134: 163,
                        135: 9834,
                        136: 224,
                        137: 32,
                        138: 232,
                        139: 226,
                        140: 234,
                        141: 238,
                        142: 244,
                        143: 251,
                        144: 193,
                        145: 201,
                        146: 211,
                        147: 218,
                        148: 220,
                        149: 252,
                        150: 8216,
                        151: 161,
                        152: 42,
                        153: 8217,
                        154: 9473,
                        155: 169,
                        156: 8480,
                        157: 8226,
                        158: 8220,
                        159: 8221,
                        160: 192,
                        161: 194,
                        162: 199,
                        163: 200,
                        164: 202,
                        165: 203,
                        166: 235,
                        167: 206,
                        168: 207,
                        169: 239,
                        170: 212,
                        171: 217,
                        172: 249,
                        173: 219,
                        174: 171,
                        175: 187,
                        176: 195,
                        177: 227,
                        178: 205,
                        179: 204,
                        180: 236,
                        181: 210,
                        182: 242,
                        183: 213,
                        184: 245,
                        185: 123,
                        186: 125,
                        187: 92,
                        188: 94,
                        189: 95,
                        190: 124,
                        191: 8764,
                        192: 196,
                        193: 228,
                        194: 214,
                        195: 246,
                        196: 223,
                        197: 165,
                        198: 164,
                        199: 9475,
                        200: 197,
                        201: 229,
                        202: 216,
                        203: 248,
                        204: 9487,
                        205: 9491,
                        206: 9495,
                        207: 9499,
                    },
                    _e = function (t) {
                        var e = t;
                        return ke.hasOwnProperty(t) && (e = ke[t]), String.fromCharCode(e);
                    },
                    Ie = { 17: 1, 18: 3, 21: 5, 22: 7, 23: 9, 16: 11, 19: 12, 20: 14 },
                    Ce = { 17: 2, 18: 4, 21: 6, 22: 8, 23: 10, 19: 13, 20: 15 },
                    we = { 25: 1, 26: 3, 29: 5, 30: 7, 31: 9, 24: 11, 27: 12, 28: 14 },
                    Oe = { 25: 2, 26: 4, 29: 6, 30: 8, 31: 10, 27: 13, 28: 15 },
                    xe = ["white", "green", "blue", "cyan", "red", "yellow", "magenta", "black", "transparent"];
                !(function (t) {
                    (t[(t.ERROR = 0)] = "ERROR"), (t[(t.TEXT = 1)] = "TEXT"), (t[(t.WARNING = 2)] = "WARNING"), (t[(t.INFO = 2)] = "INFO"), (t[(t.DEBUG = 3)] = "DEBUG"), (t[(t.DATA = 3)] = "DATA");
                })(Ee || (Ee = {}));
                var Pe = (function () {
                        function t() {
                            (this.time = null), (this.verboseLevel = Ee.ERROR);
                        }
                        return (
                            (t.prototype.log = function (t, e) {
                                this.verboseLevel >= t && o.b.log(this.time + " [" + t + "] " + e);
                            }),
                            t
                        );
                    })(),
                    Me = function (t) {
                        for (var e = [], r = 0; r < t.length; r++) e.push(t[r].toString(16));
                        return e;
                    },
                    Fe = (function () {
                        function t(t, e, r, i, n) {
                            (this.foreground = void 0),
                                (this.underline = void 0),
                                (this.italics = void 0),
                                (this.background = void 0),
                                (this.flash = void 0),
                                (this.foreground = t || "white"),
                                (this.underline = e || !1),
                                (this.italics = r || !1),
                                (this.background = i || "black"),
                                (this.flash = n || !1);
                        }
                        var e = t.prototype;
                        return (
                            (e.reset = function () {
                                (this.foreground = "white"), (this.underline = !1), (this.italics = !1), (this.background = "black"), (this.flash = !1);
                            }),
                            (e.setStyles = function (t) {
                                for (var e = ["foreground", "underline", "italics", "background", "flash"], r = 0; r < e.length; r++) {
                                    var i = e[r];
                                    t.hasOwnProperty(i) && (this[i] = t[i]);
                                }
                            }),
                            (e.isDefault = function () {
                                return "white" === this.foreground && !this.underline && !this.italics && "black" === this.background && !this.flash;
                            }),
                            (e.equals = function (t) {
                                return this.foreground === t.foreground && this.underline === t.underline && this.italics === t.italics && this.background === t.background && this.flash === t.flash;
                            }),
                            (e.copy = function (t) {
                                (this.foreground = t.foreground), (this.underline = t.underline), (this.italics = t.italics), (this.background = t.background), (this.flash = t.flash);
                            }),
                            (e.toString = function () {
                                return "color=" + this.foreground + ", underline=" + this.underline + ", italics=" + this.italics + ", background=" + this.background + ", flash=" + this.flash;
                            }),
                            t
                        );
                    })(),
                    Ne = (function () {
                        function t(t, e, r, i, n, a) {
                            (this.uchar = void 0), (this.penState = void 0), (this.uchar = t || " "), (this.penState = new Fe(e, r, i, n, a));
                        }
                        var e = t.prototype;
                        return (
                            (e.reset = function () {
                                (this.uchar = " "), this.penState.reset();
                            }),
                            (e.setChar = function (t, e) {
                                (this.uchar = t), this.penState.copy(e);
                            }),
                            (e.setPenState = function (t) {
                                this.penState.copy(t);
                            }),
                            (e.equals = function (t) {
                                return this.uchar === t.uchar && this.penState.equals(t.penState);
                            }),
                            (e.copy = function (t) {
                                (this.uchar = t.uchar), this.penState.copy(t.penState);
                            }),
                            (e.isEmpty = function () {
                                return " " === this.uchar && this.penState.isDefault();
                            }),
                            t
                        );
                    })(),
                    Ue = (function () {
                        function t(t) {
                            (this.chars = void 0), (this.pos = void 0), (this.currPenState = void 0), (this.cueStartTime = void 0), (this.logger = void 0), (this.chars = []);
                            for (var e = 0; e < 100; e++) this.chars.push(new Ne());
                            (this.logger = t), (this.pos = 0), (this.currPenState = new Fe());
                        }
                        var e = t.prototype;
                        return (
                            (e.equals = function (t) {
                                for (var e = !0, r = 0; r < 100; r++)
                                    if (!this.chars[r].equals(t.chars[r])) {
                                        e = !1;
                                        break;
                                    }
                                return e;
                            }),
                            (e.copy = function (t) {
                                for (var e = 0; e < 100; e++) this.chars[e].copy(t.chars[e]);
                            }),
                            (e.isEmpty = function () {
                                for (var t = !0, e = 0; e < 100; e++)
                                    if (!this.chars[e].isEmpty()) {
                                        t = !1;
                                        break;
                                    }
                                return t;
                            }),
                            (e.setCursor = function (t) {
                                this.pos !== t && (this.pos = t),
                                    this.pos < 0
                                        ? (this.logger.log(Ee.DEBUG, "Negative cursor position " + this.pos), (this.pos = 0))
                                        : this.pos > 100 && (this.logger.log(Ee.DEBUG, "Too large cursor position " + this.pos), (this.pos = 100));
                            }),
                            (e.moveCursor = function (t) {
                                var e = this.pos + t;
                                if (t > 1) for (var r = this.pos + 1; r < e + 1; r++) this.chars[r].setPenState(this.currPenState);
                                this.setCursor(e);
                            }),
                            (e.backSpace = function () {
                                this.moveCursor(-1), this.chars[this.pos].setChar(" ", this.currPenState);
                            }),
                            (e.insertChar = function (t) {
                                t >= 144 && this.backSpace();
                                var e = _e(t);
                                this.pos >= 100
                                    ? this.logger.log(Ee.ERROR, "Cannot insert " + t.toString(16) + " (" + e + ") at position " + this.pos + ". Skipping it!")
                                    : (this.chars[this.pos].setChar(e, this.currPenState), this.moveCursor(1));
                            }),
                            (e.clearFromPos = function (t) {
                                var e;
                                for (e = t; e < 100; e++) this.chars[e].reset();
                            }),
                            (e.clear = function () {
                                this.clearFromPos(0), (this.pos = 0), this.currPenState.reset();
                            }),
                            (e.clearToEndOfRow = function () {
                                this.clearFromPos(this.pos);
                            }),
                            (e.getTextString = function () {
                                for (var t = [], e = !0, r = 0; r < 100; r++) {
                                    var i = this.chars[r].uchar;
                                    " " !== i && (e = !1), t.push(i);
                                }
                                return e ? "" : t.join("");
                            }),
                            (e.setPenStyles = function (t) {
                                this.currPenState.setStyles(t), this.chars[this.pos].setPenState(this.currPenState);
                            }),
                            t
                        );
                    })(),
                    Be = (function () {
                        function t(t) {
                            (this.rows = void 0), (this.currRow = void 0), (this.nrRollUpRows = void 0), (this.lastOutputScreen = void 0), (this.logger = void 0), (this.rows = []);
                            for (var e = 0; e < 15; e++) this.rows.push(new Ue(t));
                            (this.logger = t), (this.currRow = 14), (this.nrRollUpRows = null), (this.lastOutputScreen = null), this.reset();
                        }
                        var e = t.prototype;
                        return (
                            (e.reset = function () {
                                for (var t = 0; t < 15; t++) this.rows[t].clear();
                                this.currRow = 14;
                            }),
                            (e.equals = function (t) {
                                for (var e = !0, r = 0; r < 15; r++)
                                    if (!this.rows[r].equals(t.rows[r])) {
                                        e = !1;
                                        break;
                                    }
                                return e;
                            }),
                            (e.copy = function (t) {
                                for (var e = 0; e < 15; e++) this.rows[e].copy(t.rows[e]);
                            }),
                            (e.isEmpty = function () {
                                for (var t = !0, e = 0; e < 15; e++)
                                    if (!this.rows[e].isEmpty()) {
                                        t = !1;
                                        break;
                                    }
                                return t;
                            }),
                            (e.backSpace = function () {
                                this.rows[this.currRow].backSpace();
                            }),
                            (e.clearToEndOfRow = function () {
                                this.rows[this.currRow].clearToEndOfRow();
                            }),
                            (e.insertChar = function (t) {
                                this.rows[this.currRow].insertChar(t);
                            }),
                            (e.setPen = function (t) {
                                this.rows[this.currRow].setPenStyles(t);
                            }),
                            (e.moveCursor = function (t) {
                                this.rows[this.currRow].moveCursor(t);
                            }),
                            (e.setCursor = function (t) {
                                this.logger.log(Ee.INFO, "setCursor: " + t), this.rows[this.currRow].setCursor(t);
                            }),
                            (e.setPAC = function (t) {
                                this.logger.log(Ee.INFO, "pacData = " + JSON.stringify(t));
                                var e = t.row - 1;
                                if ((this.nrRollUpRows && e < this.nrRollUpRows - 1 && (e = this.nrRollUpRows - 1), this.nrRollUpRows && this.currRow !== e)) {
                                    for (var r = 0; r < 15; r++) this.rows[r].clear();
                                    var i = this.currRow + 1 - this.nrRollUpRows,
                                        n = this.lastOutputScreen;
                                    if (n) {
                                        var a = n.rows[i].cueStartTime,
                                            s = this.logger.time;
                                        if (a && null !== s && a < s) for (var o = 0; o < this.nrRollUpRows; o++) this.rows[e - this.nrRollUpRows + o + 1].copy(n.rows[i + o]);
                                    }
                                }
                                this.currRow = e;
                                var l = this.rows[this.currRow];
                                if (null !== t.indent) {
                                    var u = t.indent,
                                        h = Math.max(u - 1, 0);
                                    l.setCursor(t.indent), (t.color = l.chars[h].penState.foreground);
                                }
                                var d = { foreground: t.color, underline: t.underline, italics: t.italics, background: "black", flash: !1 };
                                this.setPen(d);
                            }),
                            (e.setBkgData = function (t) {
                                this.logger.log(Ee.INFO, "bkgData = " + JSON.stringify(t)), this.backSpace(), this.setPen(t), this.insertChar(32);
                            }),
                            (e.setRollUpRows = function (t) {
                                this.nrRollUpRows = t;
                            }),
                            (e.rollUp = function () {
                                if (null !== this.nrRollUpRows) {
                                    this.logger.log(Ee.TEXT, this.getDisplayText());
                                    var t = this.currRow + 1 - this.nrRollUpRows,
                                        e = this.rows.splice(t, 1)[0];
                                    e.clear(), this.rows.splice(this.currRow, 0, e), this.logger.log(Ee.INFO, "Rolling up");
                                } else this.logger.log(Ee.DEBUG, "roll_up but nrRollUpRows not set yet");
                            }),
                            (e.getDisplayText = function (t) {
                                t = t || !1;
                                for (var e = [], r = "", i = -1, n = 0; n < 15; n++) {
                                    var a = this.rows[n].getTextString();
                                    a && ((i = n + 1), t ? e.push("Row " + i + ": '" + a + "'") : e.push(a.trim()));
                                }
                                return e.length > 0 && (r = t ? "[" + e.join(" | ") + "]" : e.join("\n")), r;
                            }),
                            (e.getTextAndFormat = function () {
                                return this.rows;
                            }),
                            t
                        );
                    })(),
                    Ge = (function () {
                        function t(t, e, r) {
                            (this.chNr = void 0),
                                (this.outputFilter = void 0),
                                (this.mode = void 0),
                                (this.verbose = void 0),
                                (this.displayedMemory = void 0),
                                (this.nonDisplayedMemory = void 0),
                                (this.lastOutputScreen = void 0),
                                (this.currRollUpRow = void 0),
                                (this.writeScreen = void 0),
                                (this.cueStartTime = void 0),
                                (this.logger = void 0),
                                (this.chNr = t),
                                (this.outputFilter = e),
                                (this.mode = null),
                                (this.verbose = 0),
                                (this.displayedMemory = new Be(r)),
                                (this.nonDisplayedMemory = new Be(r)),
                                (this.lastOutputScreen = new Be(r)),
                                (this.currRollUpRow = this.displayedMemory.rows[14]),
                                (this.writeScreen = this.displayedMemory),
                                (this.mode = null),
                                (this.cueStartTime = null),
                                (this.logger = r);
                        }
                        var e = t.prototype;
                        return (
                            (e.reset = function () {
                                (this.mode = null),
                                    this.displayedMemory.reset(),
                                    this.nonDisplayedMemory.reset(),
                                    this.lastOutputScreen.reset(),
                                    this.outputFilter.reset(),
                                    (this.currRollUpRow = this.displayedMemory.rows[14]),
                                    (this.writeScreen = this.displayedMemory),
                                    (this.mode = null),
                                    (this.cueStartTime = null);
                            }),
                            (e.getHandler = function () {
                                return this.outputFilter;
                            }),
                            (e.setHandler = function (t) {
                                this.outputFilter = t;
                            }),
                            (e.setPAC = function (t) {
                                this.writeScreen.setPAC(t);
                            }),
                            (e.setBkgData = function (t) {
                                this.writeScreen.setBkgData(t);
                            }),
                            (e.setMode = function (t) {
                                t !== this.mode &&
                                    ((this.mode = t),
                                    this.logger.log(Ee.INFO, "MODE=" + t),
                                    "MODE_POP-ON" === this.mode ? (this.writeScreen = this.nonDisplayedMemory) : ((this.writeScreen = this.displayedMemory), this.writeScreen.reset()),
                                    "MODE_ROLL-UP" !== this.mode && ((this.displayedMemory.nrRollUpRows = null), (this.nonDisplayedMemory.nrRollUpRows = null)),
                                    (this.mode = t));
                            }),
                            (e.insertChars = function (t) {
                                for (var e = 0; e < t.length; e++) this.writeScreen.insertChar(t[e]);
                                var r = this.writeScreen === this.displayedMemory ? "DISP" : "NON_DISP";
                                this.logger.log(Ee.INFO, r + ": " + this.writeScreen.getDisplayText(!0)),
                                    ("MODE_PAINT-ON" !== this.mode && "MODE_ROLL-UP" !== this.mode) || (this.logger.log(Ee.TEXT, "DISPLAYED: " + this.displayedMemory.getDisplayText(!0)), this.outputDataUpdate());
                            }),
                            (e.ccRCL = function () {
                                this.logger.log(Ee.INFO, "RCL - Resume Caption Loading"), this.setMode("MODE_POP-ON");
                            }),
                            (e.ccBS = function () {
                                this.logger.log(Ee.INFO, "BS - BackSpace"), "MODE_TEXT" !== this.mode && (this.writeScreen.backSpace(), this.writeScreen === this.displayedMemory && this.outputDataUpdate());
                            }),
                            (e.ccAOF = function () {}),
                            (e.ccAON = function () {}),
                            (e.ccDER = function () {
                                this.logger.log(Ee.INFO, "DER- Delete to End of Row"), this.writeScreen.clearToEndOfRow(), this.outputDataUpdate();
                            }),
                            (e.ccRU = function (t) {
                                this.logger.log(Ee.INFO, "RU(" + t + ") - Roll Up"), (this.writeScreen = this.displayedMemory), this.setMode("MODE_ROLL-UP"), this.writeScreen.setRollUpRows(t);
                            }),
                            (e.ccFON = function () {
                                this.logger.log(Ee.INFO, "FON - Flash On"), this.writeScreen.setPen({ flash: !0 });
                            }),
                            (e.ccRDC = function () {
                                this.logger.log(Ee.INFO, "RDC - Resume Direct Captioning"), this.setMode("MODE_PAINT-ON");
                            }),
                            (e.ccTR = function () {
                                this.logger.log(Ee.INFO, "TR"), this.setMode("MODE_TEXT");
                            }),
                            (e.ccRTD = function () {
                                this.logger.log(Ee.INFO, "RTD"), this.setMode("MODE_TEXT");
                            }),
                            (e.ccEDM = function () {
                                this.logger.log(Ee.INFO, "EDM - Erase Displayed Memory"), this.displayedMemory.reset(), this.outputDataUpdate(!0);
                            }),
                            (e.ccCR = function () {
                                this.logger.log(Ee.INFO, "CR - Carriage Return"), this.writeScreen.rollUp(), this.outputDataUpdate(!0);
                            }),
                            (e.ccENM = function () {
                                this.logger.log(Ee.INFO, "ENM - Erase Non-displayed Memory"), this.nonDisplayedMemory.reset();
                            }),
                            (e.ccEOC = function () {
                                if ((this.logger.log(Ee.INFO, "EOC - End Of Caption"), "MODE_POP-ON" === this.mode)) {
                                    var t = this.displayedMemory;
                                    (this.displayedMemory = this.nonDisplayedMemory), (this.nonDisplayedMemory = t), (this.writeScreen = this.nonDisplayedMemory), this.logger.log(Ee.TEXT, "DISP: " + this.displayedMemory.getDisplayText());
                                }
                                this.outputDataUpdate(!0);
                            }),
                            (e.ccTO = function (t) {
                                this.logger.log(Ee.INFO, "TO(" + t + ") - Tab Offset"), this.writeScreen.moveCursor(t);
                            }),
                            (e.ccMIDROW = function (t) {
                                var e = { flash: !1 };
                                if (((e.underline = t % 2 == 1), (e.italics = t >= 46), e.italics)) e.foreground = "white";
                                else {
                                    var r = Math.floor(t / 2) - 16;
                                    e.foreground = ["white", "green", "blue", "cyan", "red", "yellow", "magenta"][r];
                                }
                                this.logger.log(Ee.INFO, "MIDROW: " + JSON.stringify(e)), this.writeScreen.setPen(e);
                            }),
                            (e.outputDataUpdate = function (t) {
                                void 0 === t && (t = !1);
                                var e = this.logger.time;
                                null !== e &&
                                    this.outputFilter &&
                                    (null !== this.cueStartTime || this.displayedMemory.isEmpty()
                                        ? this.displayedMemory.equals(this.lastOutputScreen) ||
                                          (this.outputFilter.newCue(this.cueStartTime, e, this.lastOutputScreen),
                                          t && this.outputFilter.dispatchCue && this.outputFilter.dispatchCue(),
                                          (this.cueStartTime = this.displayedMemory.isEmpty() ? null : e))
                                        : (this.cueStartTime = e),
                                    this.lastOutputScreen.copy(this.displayedMemory));
                            }),
                            (e.cueSplitAtTime = function (t) {
                                this.outputFilter && (this.displayedMemory.isEmpty() || (this.outputFilter.newCue && this.outputFilter.newCue(this.cueStartTime, t, this.displayedMemory), (this.cueStartTime = t)));
                            }),
                            t
                        );
                    })();
                function Ke(t, e, r) {
                    (r.a = t), (r.b = e);
                }
                function je(t, e, r) {
                    return r.a === t && r.b === e;
                }
                var He = (function () {
                        function t(t, e, r) {
                            (this.channels = void 0), (this.currentChannel = 0), (this.cmdHistory = void 0), (this.logger = void 0);
                            var i = new Pe();
                            (this.channels = [null, new Ge(t, e, i), new Ge(t + 1, r, i)]), (this.cmdHistory = { a: null, b: null }), (this.logger = i);
                        }
                        var e = t.prototype;
                        return (
                            (e.getHandler = function (t) {
                                return this.channels[t].getHandler();
                            }),
                            (e.setHandler = function (t, e) {
                                this.channels[t].setHandler(e);
                            }),
                            (e.addData = function (t, e) {
                                var r,
                                    i,
                                    n,
                                    a = !1;
                                this.logger.time = t;
                                for (var s = 0; s < e.length; s += 2)
                                    if (((i = 127 & e[s]), (n = 127 & e[s + 1]), 0 !== i || 0 !== n)) {
                                        if (
                                            (this.logger.log(Ee.DATA, "[" + Me([e[s], e[s + 1]]) + "] -> (" + Me([i, n]) + ")"),
                                            (r = this.parseCmd(i, n)) || (r = this.parseMidrow(i, n)),
                                            r || (r = this.parsePAC(i, n)),
                                            r || (r = this.parseBackgroundAttributes(i, n)),
                                            !r && (a = this.parseChars(i, n)))
                                        ) {
                                            var o = this.currentChannel;
                                            if (o && o > 0) this.channels[o].insertChars(a);
                                            else this.logger.log(Ee.WARNING, "No channel found yet. TEXT-MODE?");
                                        }
                                        r || a || this.logger.log(Ee.WARNING, "Couldn't parse cleaned data " + Me([i, n]) + " orig: " + Me([e[s], e[s + 1]]));
                                    }
                            }),
                            (e.parseCmd = function (t, e) {
                                var r = this.cmdHistory;
                                if (!((20 === t || 28 === t || 21 === t || 29 === t) && e >= 32 && e <= 47) && !((23 === t || 31 === t) && e >= 33 && e <= 35)) return !1;
                                if (je(t, e, r)) return Ke(null, null, r), this.logger.log(Ee.DEBUG, "Repeated command (" + Me([t, e]) + ") is dropped"), !0;
                                var i = 20 === t || 21 === t || 23 === t ? 1 : 2,
                                    n = this.channels[i];
                                return (
                                    20 === t || 21 === t || 28 === t || 29 === t
                                        ? 32 === e
                                            ? n.ccRCL()
                                            : 33 === e
                                            ? n.ccBS()
                                            : 34 === e
                                            ? n.ccAOF()
                                            : 35 === e
                                            ? n.ccAON()
                                            : 36 === e
                                            ? n.ccDER()
                                            : 37 === e
                                            ? n.ccRU(2)
                                            : 38 === e
                                            ? n.ccRU(3)
                                            : 39 === e
                                            ? n.ccRU(4)
                                            : 40 === e
                                            ? n.ccFON()
                                            : 41 === e
                                            ? n.ccRDC()
                                            : 42 === e
                                            ? n.ccTR()
                                            : 43 === e
                                            ? n.ccRTD()
                                            : 44 === e
                                            ? n.ccEDM()
                                            : 45 === e
                                            ? n.ccCR()
                                            : 46 === e
                                            ? n.ccENM()
                                            : 47 === e && n.ccEOC()
                                        : n.ccTO(e - 32),
                                    Ke(t, e, r),
                                    (this.currentChannel = i),
                                    !0
                                );
                            }),
                            (e.parseMidrow = function (t, e) {
                                var r = 0;
                                if ((17 === t || 25 === t) && e >= 32 && e <= 47) {
                                    if ((r = 17 === t ? 1 : 2) !== this.currentChannel) return this.logger.log(Ee.ERROR, "Mismatch channel in midrow parsing"), !1;
                                    var i = this.channels[r];
                                    return !!i && (i.ccMIDROW(e), this.logger.log(Ee.DEBUG, "MIDROW (" + Me([t, e]) + ")"), !0);
                                }
                                return !1;
                            }),
                            (e.parsePAC = function (t, e) {
                                var r,
                                    i = this.cmdHistory;
                                if (!(((t >= 17 && t <= 23) || (t >= 25 && t <= 31)) && e >= 64 && e <= 127) && !((16 === t || 24 === t) && e >= 64 && e <= 95)) return !1;
                                if (je(t, e, i)) return Ke(null, null, i), !0;
                                var n = t <= 23 ? 1 : 2;
                                r = e >= 64 && e <= 95 ? (1 === n ? Ie[t] : we[t]) : 1 === n ? Ce[t] : Oe[t];
                                var a = this.channels[n];
                                return !!a && (a.setPAC(this.interpretPAC(r, e)), Ke(t, e, i), (this.currentChannel = n), !0);
                            }),
                            (e.interpretPAC = function (t, e) {
                                var r,
                                    i = { color: null, italics: !1, indent: null, underline: !1, row: t };
                                return (
                                    (r = e > 95 ? e - 96 : e - 64),
                                    (i.underline = 1 == (1 & r)),
                                    r <= 13
                                        ? (i.color = ["white", "green", "blue", "cyan", "red", "yellow", "magenta", "white"][Math.floor(r / 2)])
                                        : r <= 15
                                        ? ((i.italics = !0), (i.color = "white"))
                                        : (i.indent = 4 * Math.floor((r - 16) / 2)),
                                    i
                                );
                            }),
                            (e.parseChars = function (t, e) {
                                var r,
                                    i,
                                    n = null,
                                    a = null;
                                (t >= 25 ? ((r = 2), (a = t - 8)) : ((r = 1), (a = t)), a >= 17 && a <= 19)
                                    ? ((i = 17 === a ? e + 80 : 18 === a ? e + 112 : e + 144), this.logger.log(Ee.INFO, "Special char '" + _e(i) + "' in channel " + r), (n = [i]))
                                    : t >= 32 && t <= 127 && (n = 0 === e ? [t] : [t, e]);
                                if (n) {
                                    var s = Me(n);
                                    this.logger.log(Ee.DEBUG, "Char codes =  " + s.join(",")), Ke(t, e, this.cmdHistory);
                                }
                                return n;
                            }),
                            (e.parseBackgroundAttributes = function (t, e) {
                                var r;
                                if (!((16 === t || 24 === t) && e >= 32 && e <= 47) && !((23 === t || 31 === t) && e >= 45 && e <= 47)) return !1;
                                var i = {};
                                16 === t || 24 === t
                                    ? ((r = Math.floor((e - 32) / 2)), (i.background = xe[r]), e % 2 == 1 && (i.background = i.background + "_semi"))
                                    : 45 === e
                                    ? (i.background = "transparent")
                                    : ((i.foreground = "black"), 47 === e && (i.underline = !0));
                                var n = t <= 23 ? 1 : 2;
                                return this.channels[n].setBkgData(i), Ke(t, e, this.cmdHistory), !0;
                            }),
                            (e.reset = function () {
                                for (var t = 0; t < Object.keys(this.channels).length; t++) {
                                    var e = this.channels[t];
                                    e && e.reset();
                                }
                                this.cmdHistory = { a: null, b: null };
                            }),
                            (e.cueSplitAtTime = function (t) {
                                for (var e = 0; e < this.channels.length; e++) {
                                    var r = this.channels[e];
                                    r && r.cueSplitAtTime(t);
                                }
                            }),
                            t
                        );
                    })(),
                    Ve = (function () {
                        function t(t, e) {
                            (this.timelineController = void 0), (this.cueRanges = []), (this.trackName = void 0), (this.startTime = null), (this.endTime = null), (this.screen = null), (this.timelineController = t), (this.trackName = e);
                        }
                        var e = t.prototype;
                        return (
                            (e.dispatchCue = function () {
                                null !== this.startTime && (this.timelineController.addCues(this.trackName, this.startTime, this.endTime, this.screen, this.cueRanges), (this.startTime = null));
                            }),
                            (e.newCue = function (t, e, r) {
                                (null === this.startTime || this.startTime > t) && (this.startTime = t), (this.endTime = e), (this.screen = r), this.timelineController.createCaptionsTrack(this.trackName);
                            }),
                            (e.reset = function () {
                                this.cueRanges = [];
                            }),
                            t
                        );
                    })(),
                    We = (function () {
                        if ("undefined" != typeof self && self.VTTCue) return self.VTTCue;
                        var t = ["", "lr", "rl"],
                            e = ["start", "middle", "end", "left", "right"];
                        function r(t, e) {
                            if ("string" != typeof e) return !1;
                            if (!Array.isArray(t)) return !1;
                            var r = e.toLowerCase();
                            return !!~t.indexOf(r) && r;
                        }
                        function i(t) {
                            return r(e, t);
                        }
                        function n(t) {
                            for (var e = arguments.length, r = new Array(e > 1 ? e - 1 : 0), i = 1; i < e; i++) r[i - 1] = arguments[i];
                            for (var n = 1; n < arguments.length; n++) {
                                var a = arguments[n];
                                for (var s in a) t[s] = a[s];
                            }
                            return t;
                        }
                        function a(e, a, s) {
                            var o = this,
                                l = { enumerable: !0 };
                            o.hasBeenReset = !1;
                            var u = "",
                                h = !1,
                                d = e,
                                c = a,
                                f = s,
                                g = null,
                                v = "",
                                p = !0,
                                m = "auto",
                                y = "start",
                                T = 50,
                                b = "middle",
                                E = 50,
                                S = "middle";
                            Object.defineProperty(
                                o,
                                "id",
                                n({}, l, {
                                    get: function () {
                                        return u;
                                    },
                                    set: function (t) {
                                        u = "" + t;
                                    },
                                })
                            ),
                                Object.defineProperty(
                                    o,
                                    "pauseOnExit",
                                    n({}, l, {
                                        get: function () {
                                            return h;
                                        },
                                        set: function (t) {
                                            h = !!t;
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "startTime",
                                    n({}, l, {
                                        get: function () {
                                            return d;
                                        },
                                        set: function (t) {
                                            if ("number" != typeof t) throw new TypeError("Start time must be set to a number.");
                                            (d = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "endTime",
                                    n({}, l, {
                                        get: function () {
                                            return c;
                                        },
                                        set: function (t) {
                                            if ("number" != typeof t) throw new TypeError("End time must be set to a number.");
                                            (c = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "text",
                                    n({}, l, {
                                        get: function () {
                                            return f;
                                        },
                                        set: function (t) {
                                            (f = "" + t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "region",
                                    n({}, l, {
                                        get: function () {
                                            return g;
                                        },
                                        set: function (t) {
                                            (g = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "vertical",
                                    n({}, l, {
                                        get: function () {
                                            return v;
                                        },
                                        set: function (e) {
                                            var i = (function (e) {
                                                return r(t, e);
                                            })(e);
                                            if (!1 === i) throw new SyntaxError("An invalid or illegal string was specified.");
                                            (v = i), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "snapToLines",
                                    n({}, l, {
                                        get: function () {
                                            return p;
                                        },
                                        set: function (t) {
                                            (p = !!t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "line",
                                    n({}, l, {
                                        get: function () {
                                            return m;
                                        },
                                        set: function (t) {
                                            if ("number" != typeof t && "auto" !== t) throw new SyntaxError("An invalid number or illegal string was specified.");
                                            (m = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "lineAlign",
                                    n({}, l, {
                                        get: function () {
                                            return y;
                                        },
                                        set: function (t) {
                                            var e = i(t);
                                            if (!e) throw new SyntaxError("An invalid or illegal string was specified.");
                                            (y = e), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "position",
                                    n({}, l, {
                                        get: function () {
                                            return T;
                                        },
                                        set: function (t) {
                                            if (t < 0 || t > 100) throw new Error("Position must be between 0 and 100.");
                                            (T = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "positionAlign",
                                    n({}, l, {
                                        get: function () {
                                            return b;
                                        },
                                        set: function (t) {
                                            var e = i(t);
                                            if (!e) throw new SyntaxError("An invalid or illegal string was specified.");
                                            (b = e), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "size",
                                    n({}, l, {
                                        get: function () {
                                            return E;
                                        },
                                        set: function (t) {
                                            if (t < 0 || t > 100) throw new Error("Size must be between 0 and 100.");
                                            (E = t), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                Object.defineProperty(
                                    o,
                                    "align",
                                    n({}, l, {
                                        get: function () {
                                            return S;
                                        },
                                        set: function (t) {
                                            var e = i(t);
                                            if (!e) throw new SyntaxError("An invalid or illegal string was specified.");
                                            (S = e), (this.hasBeenReset = !0);
                                        },
                                    })
                                ),
                                (o.displayState = void 0);
                        }
                        return (
                            (a.prototype.getCueAsHTML = function () {
                                return self.WebVTT.convertCueToDOMTree(self, this.text);
                            }),
                            a
                        );
                    })(),
                    Ye = (function () {
                        function t() {}
                        return (
                            (t.prototype.decode = function (t, e) {
                                if (!t) return "";
                                if ("string" != typeof t) throw new Error("Error - expected string data.");
                                return decodeURIComponent(encodeURIComponent(t));
                            }),
                            t
                        );
                    })();
                function qe(t) {
                    function e(t, e, r, i) {
                        return 3600 * (0 | t) + 60 * (0 | e) + (0 | r) + parseFloat(i || 0);
                    }
                    var r = t.match(/^(?:(\d+):)?(\d{2}):(\d{2})(\.\d+)?/);
                    return r ? (parseFloat(r[2]) > 59 ? e(r[2], r[3], 0, r[4]) : e(r[1], r[2], r[3], r[4])) : null;
                }
                var Xe = (function () {
                    function t() {
                        this.values = Object.create(null);
                    }
                    var e = t.prototype;
                    return (
                        (e.set = function (t, e) {
                            this.get(t) || "" === e || (this.values[t] = e);
                        }),
                        (e.get = function (t, e, r) {
                            return r ? (this.has(t) ? this.values[t] : e[r]) : this.has(t) ? this.values[t] : e;
                        }),
                        (e.has = function (t) {
                            return t in this.values;
                        }),
                        (e.alt = function (t, e, r) {
                            for (var i = 0; i < r.length; ++i)
                                if (e === r[i]) {
                                    this.set(t, e);
                                    break;
                                }
                        }),
                        (e.integer = function (t, e) {
                            /^-?\d+$/.test(e) && this.set(t, parseInt(e, 10));
                        }),
                        (e.percent = function (t, e) {
                            if (/^([\d]{1,3})(\.[\d]*)?%$/.test(e)) {
                                var r = parseFloat(e);
                                if (r >= 0 && r <= 100) return this.set(t, r), !0;
                            }
                            return !1;
                        }),
                        t
                    );
                })();
                function ze(t, e, r, i) {
                    var n = i ? t.split(i) : [t];
                    for (var a in n)
                        if ("string" == typeof n[a]) {
                            var s = n[a].split(r);
                            if (2 === s.length) e(s[0], s[1]);
                        }
                }
                var Qe = new We(0, 0, ""),
                    $e = "middle" === Qe.align ? "middle" : "center";
                function Je(t, e, r) {
                    var i = t;
                    function n() {
                        var e = qe(t);
                        if (null === e) throw new Error("Malformed timestamp: " + i);
                        return (t = t.replace(/^[^\sa-zA-Z-]+/, "")), e;
                    }
                    function a() {
                        t = t.replace(/^\s+/, "");
                    }
                    if ((a(), (e.startTime = n()), a(), "--\x3e" !== t.substr(0, 3))) throw new Error("Malformed time stamp (time stamps must be separated by '--\x3e'): " + i);
                    (t = t.substr(3)),
                        a(),
                        (e.endTime = n()),
                        a(),
                        (function (t, e) {
                            var i = new Xe();
                            ze(
                                t,
                                function (t, e) {
                                    var n;
                                    switch (t) {
                                        case "region":
                                            for (var a = r.length - 1; a >= 0; a--)
                                                if (r[a].id === e) {
                                                    i.set(t, r[a].region);
                                                    break;
                                                }
                                            break;
                                        case "vertical":
                                            i.alt(t, e, ["rl", "lr"]);
                                            break;
                                        case "line":
                                            (n = e.split(",")), i.integer(t, n[0]), i.percent(t, n[0]) && i.set("snapToLines", !1), i.alt(t, n[0], ["auto"]), 2 === n.length && i.alt("lineAlign", n[1], ["start", $e, "end"]);
                                            break;
                                        case "position":
                                            (n = e.split(",")), i.percent(t, n[0]), 2 === n.length && i.alt("positionAlign", n[1], ["start", $e, "end", "line-left", "line-right", "auto"]);
                                            break;
                                        case "size":
                                            i.percent(t, e);
                                            break;
                                        case "align":
                                            i.alt(t, e, ["start", $e, "end", "left", "right"]);
                                    }
                                },
                                /:/,
                                /\s/
                            ),
                                (e.region = i.get("region", null)),
                                (e.vertical = i.get("vertical", ""));
                            var n = i.get("line", "auto");
                            "auto" === n && -1 === Qe.line && (n = -1), (e.line = n), (e.lineAlign = i.get("lineAlign", "start")), (e.snapToLines = i.get("snapToLines", !0)), (e.size = i.get("size", 100)), (e.align = i.get("align", $e));
                            var a = i.get("position", "auto");
                            "auto" === a && 50 === Qe.position && (a = "start" === e.align || "left" === e.align ? 0 : "end" === e.align || "right" === e.align ? 100 : 50), (e.position = a);
                        })(t, e);
                }
                function Ze(t) {
                    return t.replace(/<br(?: \/)?>/gi, "\n");
                }
                var tr = (function () {
                        function t() {
                            (this.state = "INITIAL"), (this.buffer = ""), (this.decoder = new Ye()), (this.regionList = []), (this.cue = null), (this.oncue = void 0), (this.onparsingerror = void 0), (this.onflush = void 0);
                        }
                        var e = t.prototype;
                        return (
                            (e.parse = function (t) {
                                var e = this;
                                function r() {
                                    var t = e.buffer,
                                        r = 0;
                                    for (t = Ze(t); r < t.length && "\r" !== t[r] && "\n" !== t[r]; ) ++r;
                                    var i = t.substr(0, r);
                                    return "\r" === t[r] && ++r, "\n" === t[r] && ++r, (e.buffer = t.substr(r)), i;
                                }
                                t && (e.buffer += e.decoder.decode(t, { stream: !0 }));
                                try {
                                    var i = "";
                                    if ("INITIAL" === e.state) {
                                        if (!/\r\n|\n/.test(e.buffer)) return this;
                                        var n = (i = r()).match(/^(ï»¿)?WEBVTT([ \t].*)?$/);
                                        if (!n || !n[0]) throw new Error("Malformed WebVTT signature.");
                                        e.state = "HEADER";
                                    }
                                    for (var a = !1; e.buffer; ) {
                                        if (!/\r\n|\n/.test(e.buffer)) return this;
                                        switch ((a ? (a = !1) : (i = r()), e.state)) {
                                            case "HEADER":
                                                /:/.test(i) ? ze(i, function (t, e) {}, /:/) : i || (e.state = "ID");
                                                continue;
                                            case "NOTE":
                                                i || (e.state = "ID");
                                                continue;
                                            case "ID":
                                                if (/^NOTE($|[ \t])/.test(i)) {
                                                    e.state = "NOTE";
                                                    break;
                                                }
                                                if (!i) continue;
                                                if (((e.cue = new We(0, 0, "")), (e.state = "CUE"), -1 === i.indexOf("--\x3e"))) {
                                                    e.cue.id = i;
                                                    continue;
                                                }
                                            case "CUE":
                                                if (!e.cue) {
                                                    e.state = "BADCUE";
                                                    continue;
                                                }
                                                try {
                                                    Je(i, e.cue, e.regionList);
                                                } catch (t) {
                                                    (e.cue = null), (e.state = "BADCUE");
                                                    continue;
                                                }
                                                e.state = "CUETEXT";
                                                continue;
                                            case "CUETEXT":
                                                var s = -1 !== i.indexOf("--\x3e");
                                                if (!i || (s && (a = !0))) {
                                                    e.oncue && e.cue && e.oncue(e.cue), (e.cue = null), (e.state = "ID");
                                                    continue;
                                                }
                                                if (null === e.cue) continue;
                                                e.cue.text && (e.cue.text += "\n"), (e.cue.text += i);
                                                continue;
                                            case "BADCUE":
                                                i || (e.state = "ID");
                                        }
                                    }
                                } catch (t) {
                                    "CUETEXT" === e.state && e.cue && e.oncue && e.oncue(e.cue), (e.cue = null), (e.state = "INITIAL" === e.state ? "BADWEBVTT" : "BADCUE");
                                }
                                return this;
                            }),
                            (e.flush = function () {
                                try {
                                    if (((this.cue || "HEADER" === this.state) && ((this.buffer += "\n\n"), this.parse()), "INITIAL" === this.state || "BADWEBVTT" === this.state)) throw new Error("Malformed WebVTT signature.");
                                } catch (t) {
                                    this.onparsingerror && this.onparsingerror(t);
                                }
                                return this.onflush && this.onflush(), this;
                            }),
                            t
                        );
                    })(),
                    er = r(8),
                    rr = r(16),
                    ir = /\r\n|\n\r|\n|\r/g,
                    nr = function (t, e, r) {
                        return void 0 === r && (r = 0), t.substr(r, e.length) === e;
                    },
                    ar = function (t) {
                        for (var e = 5381, r = t.length; r; ) e = (33 * e) ^ t.charCodeAt(--r);
                        return (e >>> 0).toString();
                    };
                function sr(t, e, r) {
                    return ar(t.toString()) + ar(e.toString()) + ar(r);
                }
                function or(t, e, r, i, a, s, o, l) {
                    var u,
                        h = new tr(),
                        d = Object(M.f)(new Uint8Array(t)).trim().replace(ir, "\n").split("\n"),
                        c = [],
                        f = Object(er.a)(e, r),
                        g = "00:00.000",
                        v = 0,
                        p = 0,
                        m = !0,
                        y = !1;
                    (h.oncue = function (t) {
                        var e = i[a],
                            r = i.ccOffset,
                            n = (v - f) / 9e4;
                        if (
                            (null != e &&
                                e.new &&
                                (void 0 !== p
                                    ? (r = i.ccOffset = e.start)
                                    : (function (t, e, r) {
                                          var i = t[e],
                                              n = t[i.prevCC];
                                          if (!n || (!n.new && i.new)) return (t.ccOffset = t.presentationOffset = i.start), void (i.new = !1);
                                          for (; null !== (a = n) && void 0 !== a && a.new; ) {
                                              var a;
                                              (t.ccOffset += i.start - n.start), (i.new = !1), (n = t[(i = n).prevCC]);
                                          }
                                          t.presentationOffset = r;
                                      })(i, a, n)),
                            n && (r = n - i.presentationOffset),
                            y)
                        ) {
                            var o = t.endTime - t.startTime,
                                l = Object(rr.b)(9e4 * (t.startTime + r - p), 9e4 * s) / 9e4;
                            (t.startTime = l), (t.endTime = l + o);
                        }
                        var u = t.text.trim();
                        (t.text = decodeURIComponent(encodeURIComponent(u))), t.id || (t.id = sr(t.startTime, t.endTime, u)), t.endTime > 0 && c.push(t);
                    }),
                        (h.onparsingerror = function (t) {
                            u = t;
                        }),
                        (h.onflush = function () {
                            u ? l(u) : o(c);
                        }),
                        d.forEach(function (t) {
                            if (m) {
                                if (nr(t, "X-TIMESTAMP-MAP=")) {
                                    (m = !1),
                                        (y = !0),
                                        t
                                            .substr(16)
                                            .split(",")
                                            .forEach(function (t) {
                                                nr(t, "LOCAL:") ? (g = t.substr(6)) : nr(t, "MPEGTS:") && (v = parseInt(t.substr(7)));
                                            });
                                    try {
                                        p =
                                            (function (t) {
                                                var e = parseInt(t.substr(-3)),
                                                    r = parseInt(t.substr(-6, 2)),
                                                    i = parseInt(t.substr(-9, 2)),
                                                    a = t.length > 9 ? parseInt(t.substr(0, t.indexOf(":"))) : 0;
                                                if (!(Object(n.a)(e) && Object(n.a)(r) && Object(n.a)(i) && Object(n.a)(a))) throw Error("Malformed X-TIMESTAMP-MAP: Local:" + t);
                                                return (e += 1e3 * r), (e += 6e4 * i), (e += 36e5 * a);
                                            })(g) / 1e3;
                                    } catch (t) {
                                        (y = !1), (u = t);
                                    }
                                    return;
                                }
                                "" === t && (m = !1);
                            }
                            h.parse(t + "\n");
                        }),
                        h.flush();
                }
                function lr() {
                    return (lr =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                var ur = /^(\d{2,}):(\d{2}):(\d{2}):(\d{2})\.?(\d+)?$/,
                    hr = /^(\d*(?:\.\d*)?)(h|m|s|ms|f|t)$/,
                    dr = { left: "start", center: "center", right: "end", start: "start", end: "end" };
                function cr(t, e, r, i, n) {
                    var a = Object(l.b)(new Uint8Array(t), ["mdat"]);
                    if (0 !== a.length) {
                        var s = a[0],
                            o = Object(M.f)(new Uint8Array(t, s.start, s.end - s.start)),
                            u = Object(er.c)(e, 1, r);
                        try {
                            i(
                                (function (t, e) {
                                    var r = new DOMParser().parseFromString(t, "text/xml").getElementsByTagName("tt")[0];
                                    if (!r) throw new Error("Invalid ttml");
                                    var i = { frameRate: 30, subFrameRate: 1, frameRateMultiplier: 0, tickRate: 0 },
                                        n = Object.keys(i).reduce(function (t, e) {
                                            return (t[e] = r.getAttribute("ttp:" + e) || i[e]), t;
                                        }, {}),
                                        a = "preserve" !== r.getAttribute("xml:space"),
                                        s = gr(fr(r, "styling", "style")),
                                        o = gr(fr(r, "layout", "region")),
                                        l = fr(r, "body", "[begin]");
                                    return [].map
                                        .call(l, function (t) {
                                            var r = (function t(e, r) {
                                                return [].slice.call(e.childNodes).reduce(function (e, i, n) {
                                                    var a;
                                                    return "br" === i.nodeName && n ? e + "\n" : null !== (a = i.childNodes) && void 0 !== a && a.length ? t(i, r) : r ? e + i.textContent.trim().replace(/\s+/g, " ") : e + i.textContent;
                                                }, "");
                                            })(t, a);
                                            if (!r || !t.hasAttribute("begin")) return null;
                                            var i = mr(t.getAttribute("begin"), n),
                                                l = mr(t.getAttribute("dur"), n),
                                                u = mr(t.getAttribute("end"), n);
                                            if (null === i) throw pr(t);
                                            if (null === u) {
                                                if (null === l) throw pr(t);
                                                u = i + l;
                                            }
                                            var h = new We(i - e, u - e, r);
                                            h.id = sr(h.startTime, h.endTime, h.text);
                                            var d = o[t.getAttribute("region")],
                                                c = s[t.getAttribute("style")];
                                            (h.position = 10), (h.size = 80);
                                            var f = (function (t, e) {
                                                    var r = "http://www.w3.org/ns/ttml#styling";
                                                    return ["displayAlign", "textAlign", "color", "backgroundColor", "fontSize", "fontFamily"].reduce(function (i, n) {
                                                        var a = vr(e, r, n) || vr(t, r, n);
                                                        return a && (i[n] = a), i;
                                                    }, {});
                                                })(d, c),
                                                g = f.textAlign;
                                            if (g) {
                                                var v = dr[g];
                                                v && (h.lineAlign = v), (h.align = g);
                                            }
                                            return lr(h, f), h;
                                        })
                                        .filter(function (t) {
                                            return null !== t;
                                        });
                                })(o, u)
                            );
                        } catch (t) {
                            n(t);
                        }
                    } else n(new Error("Could not parse IMSC1 mdat"));
                }
                function fr(t, e, r) {
                    var i = t.getElementsByTagName(e)[0];
                    return i ? [].slice.call(i.querySelectorAll(r)) : [];
                }
                function gr(t) {
                    return t.reduce(function (t, e) {
                        var r = e.getAttribute("xml:id");
                        return r && (t[r] = e), t;
                    }, {});
                }
                function vr(t, e, r) {
                    return t.hasAttributeNS(e, r) ? t.getAttributeNS(e, r) : null;
                }
                function pr(t) {
                    return new Error("Could not parse ttml timestamp " + t);
                }
                function mr(t, e) {
                    if (!t) return null;
                    var r = qe(t);
                    return (
                        null === r &&
                            (ur.test(t)
                                ? (r = (function (t, e) {
                                      var r = ur.exec(t),
                                          i = (0 | r[4]) + (0 | r[5]) / e.subFrameRate;
                                      return 3600 * (0 | r[1]) + 60 * (0 | r[2]) + (0 | r[3]) + i / e.frameRate;
                                  })(t, e))
                                : hr.test(t) &&
                                  (r = (function (t, e) {
                                      var r = hr.exec(t),
                                          i = Number(r[1]);
                                      switch (r[2]) {
                                          case "h":
                                              return 3600 * i;
                                          case "m":
                                              return 60 * i;
                                          case "ms":
                                              return 1e3 * i;
                                          case "f":
                                              return i / e.frameRate;
                                          case "t":
                                              return i / e.tickRate;
                                      }
                                      return i;
                                  })(t, e))),
                        r
                    );
                }
                var yr = (function () {
                    function t(t) {
                        if (
                            ((this.hls = void 0),
                            (this.media = null),
                            (this.config = void 0),
                            (this.enabled = !0),
                            (this.Cues = void 0),
                            (this.textTracks = []),
                            (this.tracks = []),
                            (this.initPTS = []),
                            (this.timescale = []),
                            (this.unparsedVttFrags = []),
                            (this.captionsTracks = {}),
                            (this.nonNativeCaptionsTracks = {}),
                            (this.cea608Parser1 = void 0),
                            (this.cea608Parser2 = void 0),
                            (this.lastSn = -1),
                            (this.prevCC = -1),
                            (this.vttCCs = { ccOffset: 0, presentationOffset: 0, 0: { start: 0, prevCC: -1, new: !1 } }),
                            (this.captionsProperties = void 0),
                            (this.hls = t),
                            (this.config = t.config),
                            (this.Cues = t.config.cueHandler),
                            (this.captionsProperties = {
                                textTrack1: { label: this.config.captionsTextTrack1Label, languageCode: this.config.captionsTextTrack1LanguageCode },
                                textTrack2: { label: this.config.captionsTextTrack2Label, languageCode: this.config.captionsTextTrack2LanguageCode },
                                textTrack3: { label: this.config.captionsTextTrack3Label, languageCode: this.config.captionsTextTrack3LanguageCode },
                                textTrack4: { label: this.config.captionsTextTrack4Label, languageCode: this.config.captionsTextTrack4LanguageCode },
                            }),
                            this.config.enableCEA708Captions)
                        ) {
                            var e = new Ve(this, "textTrack1"),
                                r = new Ve(this, "textTrack2"),
                                i = new Ve(this, "textTrack3"),
                                n = new Ve(this, "textTrack4");
                            (this.cea608Parser1 = new He(1, e, r)), (this.cea608Parser2 = new He(3, i, n));
                        }
                        t.on(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                            t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                            t.on(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                            t.on(a.a.MANIFEST_LOADED, this.onManifestLoaded, this),
                            t.on(a.a.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this),
                            t.on(a.a.FRAG_LOADING, this.onFragLoading, this),
                            t.on(a.a.FRAG_LOADED, this.onFragLoaded, this),
                            t.on(a.a.FRAG_PARSING_USERDATA, this.onFragParsingUserdata, this),
                            t.on(a.a.FRAG_DECRYPTED, this.onFragDecrypted, this),
                            t.on(a.a.INIT_PTS_FOUND, this.onInitPtsFound, this),
                            t.on(a.a.SUBTITLE_TRACKS_CLEARED, this.onSubtitleTracksCleared, this),
                            t.on(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this);
                    }
                    var e = t.prototype;
                    return (
                        (e.destroy = function () {
                            var t = this.hls;
                            t.off(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                                t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this),
                                t.off(a.a.MANIFEST_LOADING, this.onManifestLoading, this),
                                t.off(a.a.MANIFEST_LOADED, this.onManifestLoaded, this),
                                t.off(a.a.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this),
                                t.off(a.a.FRAG_LOADING, this.onFragLoading, this),
                                t.off(a.a.FRAG_LOADED, this.onFragLoaded, this),
                                t.off(a.a.FRAG_PARSING_USERDATA, this.onFragParsingUserdata, this),
                                t.off(a.a.FRAG_DECRYPTED, this.onFragDecrypted, this),
                                t.off(a.a.INIT_PTS_FOUND, this.onInitPtsFound, this),
                                t.off(a.a.SUBTITLE_TRACKS_CLEARED, this.onSubtitleTracksCleared, this),
                                t.off(a.a.BUFFER_FLUSHING, this.onBufferFlushing, this),
                                (this.hls = this.config = this.cea608Parser1 = this.cea608Parser2 = null);
                        }),
                        (e.addCues = function (t, e, r, i, n) {
                            for (var s, o, l, u, h = !1, d = n.length; d--; ) {
                                var c = n[d],
                                    f = ((s = c[0]), (o = c[1]), (l = e), (u = r), Math.min(o, u) - Math.max(s, l));
                                if (f >= 0 && ((c[0] = Math.min(c[0], e)), (c[1] = Math.max(c[1], r)), (h = !0), f / (r - e) > 0.5)) return;
                            }
                            if ((h || n.push([e, r]), this.config.renderTextTracksNatively)) {
                                var g = this.captionsTracks[t];
                                this.Cues.newCue(g, e, r, i);
                            } else {
                                var v = this.Cues.newCue(null, e, r, i);
                                this.hls.trigger(a.a.CUES_PARSED, { type: "captions", cues: v, track: t });
                            }
                        }),
                        (e.onInitPtsFound = function (t, e) {
                            var r = this,
                                i = e.frag,
                                n = e.id,
                                s = e.initPTS,
                                o = e.timescale,
                                l = this.unparsedVttFrags;
                            "main" === n && ((this.initPTS[i.cc] = s), (this.timescale[i.cc] = o)),
                                l.length &&
                                    ((this.unparsedVttFrags = []),
                                    l.forEach(function (t) {
                                        r.onFragLoaded(a.a.FRAG_LOADED, t);
                                    }));
                        }),
                        (e.getExistingTrack = function (t) {
                            var e = this.media;
                            if (e)
                                for (var r = 0; r < e.textTracks.length; r++) {
                                    var i = e.textTracks[r];
                                    if (i[t]) return i;
                                }
                            return null;
                        }),
                        (e.createCaptionsTrack = function (t) {
                            this.config.renderTextTracksNatively ? this.createNativeTrack(t) : this.createNonNativeTrack(t);
                        }),
                        (e.createNativeTrack = function (t) {
                            if (!this.captionsTracks[t]) {
                                var e = this.captionsProperties,
                                    r = this.captionsTracks,
                                    i = this.media,
                                    n = e[t],
                                    a = n.label,
                                    s = n.languageCode,
                                    o = this.getExistingTrack(t);
                                if (o) (r[t] = o), x(r[t]), w(r[t], i);
                                else {
                                    var l = this.createTextTrack("captions", a, s);
                                    l && ((l[t] = !0), (r[t] = l));
                                }
                            }
                        }),
                        (e.createNonNativeTrack = function (t) {
                            if (!this.nonNativeCaptionsTracks[t]) {
                                var e = this.captionsProperties[t];
                                if (e) {
                                    var r = { _id: t, label: e.label, kind: "captions", default: !!e.media && !!e.media.default, closedCaptions: e.media };
                                    (this.nonNativeCaptionsTracks[t] = r), this.hls.trigger(a.a.NON_NATIVE_TEXT_TRACKS_FOUND, { tracks: [r] });
                                }
                            }
                        }),
                        (e.createTextTrack = function (t, e, r) {
                            var i = this.media;
                            if (i) return i.addTextTrack(t, e, r);
                        }),
                        (e.onMediaAttaching = function (t, e) {
                            (this.media = e.media), this._cleanTracks();
                        }),
                        (e.onMediaDetaching = function () {
                            var t = this.captionsTracks;
                            Object.keys(t).forEach(function (e) {
                                x(t[e]), delete t[e];
                            }),
                                (this.nonNativeCaptionsTracks = {});
                        }),
                        (e.onManifestLoading = function () {
                            (this.lastSn = -1),
                                (this.prevCC = -1),
                                (this.vttCCs = { ccOffset: 0, presentationOffset: 0, 0: { start: 0, prevCC: -1, new: !1 } }),
                                this._cleanTracks(),
                                (this.tracks = []),
                                (this.captionsTracks = {}),
                                (this.nonNativeCaptionsTracks = {}),
                                (this.textTracks = []),
                                (this.unparsedVttFrags = this.unparsedVttFrags || []),
                                (this.initPTS = []),
                                (this.timescale = []),
                                this.cea608Parser1 && this.cea608Parser2 && (this.cea608Parser1.reset(), this.cea608Parser2.reset());
                        }),
                        (e._cleanTracks = function () {
                            var t = this.media;
                            if (t) {
                                var e = t.textTracks;
                                if (e) for (var r = 0; r < e.length; r++) x(e[r]);
                            }
                        }),
                        (e.onSubtitleTracksUpdated = function (t, e) {
                            var r = this;
                            this.textTracks = [];
                            var i = e.subtitleTracks || [],
                                n = i.some(function (t) {
                                    return "stpp.ttml.im1t" === t.textCodec;
                                });
                            if (this.config.enableWebVTT || (n && this.config.enableIMSC1)) {
                                var s = this.tracks && i && this.tracks.length === i.length;
                                if (((this.tracks = i || []), this.config.renderTextTracksNatively)) {
                                    var o = this.media ? this.media.textTracks : [];
                                    this.tracks.forEach(function (t, e) {
                                        var i;
                                        if (e < o.length) {
                                            for (var n = null, a = 0; a < o.length; a++)
                                                if (Tr(o[a], t)) {
                                                    n = o[a];
                                                    break;
                                                }
                                            n && (i = n);
                                        }
                                        i ? x(i) : (i = r.createTextTrack("subtitles", t.name, t.lang)) && (i.mode = "disabled"), i && ((i.groupId = t.groupId), r.textTracks.push(i));
                                    });
                                } else if (!s && this.tracks && this.tracks.length) {
                                    var l = this.tracks.map(function (t) {
                                        return { label: t.name, kind: t.type.toLowerCase(), default: t.default, subtitleTrack: t };
                                    });
                                    this.hls.trigger(a.a.NON_NATIVE_TEXT_TRACKS_FOUND, { tracks: l });
                                }
                            }
                        }),
                        (e.onManifestLoaded = function (t, e) {
                            var r = this;
                            this.config.enableCEA708Captions &&
                                e.captions &&
                                e.captions.forEach(function (t) {
                                    var e = /(?:CC|SERVICE)([1-4])/.exec(t.instreamId);
                                    if (e) {
                                        var i = "textTrack" + e[1],
                                            n = r.captionsProperties[i];
                                        n && ((n.label = t.name), t.lang && (n.languageCode = t.lang), (n.media = t));
                                    }
                                });
                        }),
                        (e.onFragLoading = function (t, e) {
                            var r = this.cea608Parser1,
                                i = this.cea608Parser2,
                                n = this.lastSn;
                            if (this.enabled && r && i && e.frag.type === k.b.MAIN) {
                                var a = e.frag.sn;
                                a !== n + 1 && (r.reset(), i.reset()), (this.lastSn = a);
                            }
                        }),
                        (e.onFragLoaded = function (t, e) {
                            var r = e.frag,
                                i = e.payload,
                                s = this.initPTS,
                                o = this.unparsedVttFrags;
                            if (r.type === k.b.SUBTITLE)
                                if (i.byteLength) {
                                    if (!Object(n.a)(s[r.cc])) return o.push(e), void (s.length && this.hls.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !1, frag: r, error: new Error("Missing initial subtitle PTS") }));
                                    var l = r.decryptdata;
                                    if (null == l || null == l.key || "AES-128" !== l.method) {
                                        var u = this.tracks[r.level],
                                            h = this.vttCCs;
                                        h[r.cc] || ((h[r.cc] = { start: r.start, prevCC: this.prevCC, new: !0 }), (this.prevCC = r.cc)), u && "stpp.ttml.im1t" === u.textCodec ? this._parseIMSC1(r, i) : this._parseVTTs(r, i, h);
                                    }
                                } else this.hls.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !1, frag: r, error: new Error("Empty subtitle payload") });
                        }),
                        (e._parseIMSC1 = function (t, e) {
                            var r = this,
                                i = this.hls;
                            cr(
                                e,
                                this.initPTS[t.cc],
                                this.timescale[t.cc],
                                function (e) {
                                    r._appendCues(e, t.level), i.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !0, frag: t });
                                },
                                function (e) {
                                    o.b.log("Failed to parse IMSC1: " + e), i.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !1, frag: t, error: e });
                                }
                            );
                        }),
                        (e._parseVTTs = function (t, e, r) {
                            var i = this,
                                n = this.hls;
                            or(
                                e,
                                this.initPTS[t.cc],
                                this.timescale[t.cc],
                                r,
                                t.cc,
                                t.start,
                                function (e) {
                                    i._appendCues(e, t.level), n.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !0, frag: t });
                                },
                                function (r) {
                                    i._fallbackToIMSC1(t, e), o.b.log("Failed to parse VTT cue: " + r), n.trigger(a.a.SUBTITLE_FRAG_PROCESSED, { success: !1, frag: t, error: r });
                                }
                            );
                        }),
                        (e._fallbackToIMSC1 = function (t, e) {
                            var r = this,
                                i = this.tracks[t.level];
                            i.textCodec ||
                                cr(
                                    e,
                                    this.initPTS[t.cc],
                                    this.timescale[t.cc],
                                    function () {
                                        (i.textCodec = "stpp.ttml.im1t"), r._parseIMSC1(t, e);
                                    },
                                    function () {
                                        i.textCodec = "wvtt";
                                    }
                                );
                        }),
                        (e._appendCues = function (t, e) {
                            var r = this.hls;
                            if (this.config.renderTextTracksNatively) {
                                var i = this.textTracks[e];
                                if ("disabled" === i.mode) return;
                                t.forEach(function (t) {
                                    return O(i, t);
                                });
                            } else {
                                var n = this.tracks[e].default ? "default" : "subtitles" + e;
                                r.trigger(a.a.CUES_PARSED, { type: "subtitles", cues: t, track: n });
                            }
                        }),
                        (e.onFragDecrypted = function (t, e) {
                            var r = e.frag;
                            if (r.type === k.b.SUBTITLE) {
                                if (!Object(n.a)(this.initPTS[r.cc])) return void this.unparsedVttFrags.push(e);
                                this.onFragLoaded(a.a.FRAG_LOADED, e);
                            }
                        }),
                        (e.onSubtitleTracksCleared = function () {
                            (this.tracks = []), (this.captionsTracks = {});
                        }),
                        (e.onFragParsingUserdata = function (t, e) {
                            var r = this.cea608Parser1,
                                i = this.cea608Parser2;
                            if (this.enabled && r && i)
                                for (var n = 0; n < e.samples.length; n++) {
                                    var a = e.samples[n].bytes;
                                    if (a) {
                                        var s = this.extractCea608Data(a);
                                        r.addData(e.samples[n].pts, s[0]), i.addData(e.samples[n].pts, s[1]);
                                    }
                                }
                        }),
                        (e.onBufferFlushing = function (t, e) {
                            var r = e.startOffset,
                                i = e.endOffset,
                                n = e.endOffsetSubtitles,
                                a = e.type,
                                s = this.media;
                            if (s && !(s.currentTime < i)) {
                                if (!a || "video" === a) {
                                    var o = this.captionsTracks;
                                    Object.keys(o).forEach(function (t) {
                                        return P(o[t], r, i);
                                    });
                                }
                                if (this.config.renderTextTracksNatively && 0 === r && void 0 !== n) {
                                    var l = this.textTracks;
                                    Object.keys(l).forEach(function (t) {
                                        return P(l[t], r, n);
                                    });
                                }
                            }
                        }),
                        (e.extractCea608Data = function (t) {
                            for (var e = 31 & t[0], r = 2, i = [[], []], n = 0; n < e; n++) {
                                var a = t[r++],
                                    s = 127 & t[r++],
                                    o = 127 & t[r++],
                                    l = 3 & a;
                                (0 === s && 0 === o) || (0 != (4 & a) && ((0 !== l && 1 !== l) || (i[l].push(s), i[l].push(o))));
                            }
                            return i;
                        }),
                        t
                    );
                })();
                function Tr(t, e) {
                    return t && t.label === e.name && !(t.textTrack1 || t.textTrack2);
                }
                function br(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var Er,
                    Sr = (function () {
                        function t(t) {
                            (this.autoLevelCapping = void 0),
                                (this.firstLevel = void 0),
                                (this.media = void 0),
                                (this.restrictedLevels = void 0),
                                (this.timer = void 0),
                                (this.hls = void 0),
                                (this.streamController = void 0),
                                (this.clientRect = void 0),
                                (this.hls = t),
                                (this.autoLevelCapping = Number.POSITIVE_INFINITY),
                                (this.firstLevel = -1),
                                (this.media = null),
                                (this.restrictedLevels = []),
                                (this.timer = void 0),
                                (this.clientRect = null),
                                this.registerListeners();
                        }
                        var e,
                            r,
                            i,
                            n = t.prototype;
                        return (
                            (n.setStreamController = function (t) {
                                this.streamController = t;
                            }),
                            (n.destroy = function () {
                                this.unregisterListener(), this.hls.config.capLevelToPlayerSize && this.stopCapping(), (this.media = null), (this.clientRect = null), (this.hls = this.streamController = null);
                            }),
                            (n.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.FPS_DROP_LEVEL_CAPPING, this.onFpsDropLevelCapping, this),
                                    t.on(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                                    t.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.on(a.a.BUFFER_CODECS, this.onBufferCodecs, this),
                                    t.on(a.a.MEDIA_DETACHING, this.onMediaDetaching, this);
                            }),
                            (n.unregisterListener = function () {
                                var t = this.hls;
                                t.off(a.a.FPS_DROP_LEVEL_CAPPING, this.onFpsDropLevelCapping, this),
                                    t.off(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this),
                                    t.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this),
                                    t.off(a.a.BUFFER_CODECS, this.onBufferCodecs, this),
                                    t.off(a.a.MEDIA_DETACHING, this.onMediaDetaching, this);
                            }),
                            (n.onFpsDropLevelCapping = function (e, r) {
                                t.isLevelAllowed(r.droppedLevel, this.restrictedLevels) && this.restrictedLevels.push(r.droppedLevel);
                            }),
                            (n.onMediaAttaching = function (t, e) {
                                this.media = e.media instanceof HTMLVideoElement ? e.media : null;
                            }),
                            (n.onManifestParsed = function (t, e) {
                                var r = this.hls;
                                (this.restrictedLevels = []), (this.firstLevel = e.firstLevel), r.config.capLevelToPlayerSize && e.video && this.startCapping();
                            }),
                            (n.onBufferCodecs = function (t, e) {
                                this.hls.config.capLevelToPlayerSize && e.video && this.startCapping();
                            }),
                            (n.onMediaDetaching = function () {
                                this.stopCapping();
                            }),
                            (n.detectPlayerSize = function () {
                                if (this.media && this.mediaHeight > 0 && this.mediaWidth > 0) {
                                    var t = this.hls.levels;
                                    if (t.length) {
                                        var e = this.hls;
                                        (e.autoLevelCapping = this.getMaxLevel(t.length - 1)),
                                            e.autoLevelCapping > this.autoLevelCapping && this.streamController && this.streamController.nextLevelSwitch(),
                                            (this.autoLevelCapping = e.autoLevelCapping);
                                    }
                                }
                            }),
                            (n.getMaxLevel = function (e) {
                                var r = this,
                                    i = this.hls.levels;
                                if (!i.length) return -1;
                                var n = i.filter(function (i, n) {
                                    return t.isLevelAllowed(n, r.restrictedLevels) && n <= e;
                                });
                                return (this.clientRect = null), t.getMaxLevelByMediaSize(n, this.mediaWidth, this.mediaHeight);
                            }),
                            (n.startCapping = function () {
                                this.timer ||
                                    ((this.autoLevelCapping = Number.POSITIVE_INFINITY),
                                    (this.hls.firstLevel = this.getMaxLevel(this.firstLevel)),
                                    self.clearInterval(this.timer),
                                    (this.timer = self.setInterval(this.detectPlayerSize.bind(this), 1e3)),
                                    this.detectPlayerSize());
                            }),
                            (n.stopCapping = function () {
                                (this.restrictedLevels = []), (this.firstLevel = -1), (this.autoLevelCapping = Number.POSITIVE_INFINITY), this.timer && (self.clearInterval(this.timer), (this.timer = void 0));
                            }),
                            (n.getDimensions = function () {
                                if (this.clientRect) return this.clientRect;
                                var t = this.media,
                                    e = { width: 0, height: 0 };
                                if (t) {
                                    var r = t.getBoundingClientRect();
                                    (e.width = r.width), (e.height = r.height), e.width || e.height || ((e.width = r.right - r.left || t.width || 0), (e.height = r.bottom - r.top || t.height || 0));
                                }
                                return (this.clientRect = e), e;
                            }),
                            (t.isLevelAllowed = function (t, e) {
                                return void 0 === e && (e = []), -1 === e.indexOf(t);
                            }),
                            (t.getMaxLevelByMediaSize = function (t, e, r) {
                                if (!t || !t.length) return -1;
                                for (var i, n, a = t.length - 1, s = 0; s < t.length; s += 1) {
                                    var o = t[s];
                                    if ((o.width >= e || o.height >= r) && ((i = o), !(n = t[s + 1]) || i.width !== n.width || i.height !== n.height)) {
                                        a = s;
                                        break;
                                    }
                                }
                                return a;
                            }),
                            (e = t),
                            (i = [
                                {
                                    key: "contentScaleFactor",
                                    get: function () {
                                        var t = 1;
                                        try {
                                            t = self.devicePixelRatio;
                                        } catch (t) {}
                                        return t;
                                    },
                                },
                            ]),
                            (r = [
                                {
                                    key: "mediaWidth",
                                    get: function () {
                                        return this.getDimensions().width * t.contentScaleFactor;
                                    },
                                },
                                {
                                    key: "mediaHeight",
                                    get: function () {
                                        return this.getDimensions().height * t.contentScaleFactor;
                                    },
                                },
                            ]) && br(e.prototype, r),
                            i && br(e, i),
                            t
                        );
                    })(),
                    Lr = (function () {
                        function t(t) {
                            (this.hls = void 0),
                                (this.isVideoPlaybackQualityAvailable = !1),
                                (this.timer = void 0),
                                (this.media = null),
                                (this.lastTime = void 0),
                                (this.lastDroppedFrames = 0),
                                (this.lastDecodedFrames = 0),
                                (this.streamController = void 0),
                                (this.hls = t),
                                this.registerListeners();
                        }
                        var e = t.prototype;
                        return (
                            (e.setStreamController = function (t) {
                                this.streamController = t;
                            }),
                            (e.registerListeners = function () {
                                this.hls.on(a.a.MEDIA_ATTACHING, this.onMediaAttaching, this);
                            }),
                            (e.unregisterListeners = function () {
                                this.hls.off(a.a.MEDIA_ATTACHING, this.onMediaAttaching);
                            }),
                            (e.destroy = function () {
                                this.timer && clearInterval(this.timer), this.unregisterListeners(), (this.isVideoPlaybackQualityAvailable = !1), (this.media = null);
                            }),
                            (e.onMediaAttaching = function (t, e) {
                                var r = this.hls.config;
                                if (r.capLevelOnFPSDrop) {
                                    var i = e.media instanceof self.HTMLVideoElement ? e.media : null;
                                    (this.media = i),
                                        i && "function" == typeof i.getVideoPlaybackQuality && (this.isVideoPlaybackQualityAvailable = !0),
                                        self.clearInterval(this.timer),
                                        (this.timer = self.setInterval(this.checkFPSInterval.bind(this), r.fpsDroppedMonitoringPeriod));
                                }
                            }),
                            (e.checkFPS = function (t, e, r) {
                                var i = performance.now();
                                if (e) {
                                    if (this.lastTime) {
                                        var n = i - this.lastTime,
                                            s = r - this.lastDroppedFrames,
                                            l = e - this.lastDecodedFrames,
                                            u = (1e3 * s) / n,
                                            h = this.hls;
                                        if ((h.trigger(a.a.FPS_DROP, { currentDropped: s, currentDecoded: l, totalDroppedFrames: r }), u > 0 && s > h.config.fpsDroppedMonitoringThreshold * l)) {
                                            var d = h.currentLevel;
                                            o.b.warn("drop FPS ratio greater than max allowed value for currentLevel: " + d),
                                                d > 0 &&
                                                    (-1 === h.autoLevelCapping || h.autoLevelCapping >= d) &&
                                                    ((d -= 1), h.trigger(a.a.FPS_DROP_LEVEL_CAPPING, { level: d, droppedLevel: h.currentLevel }), (h.autoLevelCapping = d), this.streamController.nextLevelSwitch());
                                        }
                                    }
                                    (this.lastTime = i), (this.lastDroppedFrames = r), (this.lastDecodedFrames = e);
                                }
                            }),
                            (e.checkFPSInterval = function () {
                                var t = this.media;
                                if (t)
                                    if (this.isVideoPlaybackQualityAvailable) {
                                        var e = t.getVideoPlaybackQuality();
                                        this.checkFPS(t, e.totalVideoFrames, e.droppedVideoFrames);
                                    } else this.checkFPS(t, t.webkitDecodedFrameCount, t.webkitDroppedFrameCount);
                            }),
                            t
                        );
                    })();
                !(function (t) {
                    (t.WIDEVINE = "com.widevine.alpha"), (t.PLAYREADY = "com.microsoft.playready");
                })(Er || (Er = {}));
                var Ar = "undefined" != typeof self && self.navigator && self.navigator.requestMediaKeySystemAccess ? self.navigator.requestMediaKeySystemAccess.bind(self.navigator) : null;
                function Rr(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var Dr,
                    kr,
                    _r,
                    Ir = (function () {
                        function t(t) {
                            (this.hls = void 0),
                                (this._widevineLicenseUrl = void 0),
                                (this._licenseXhrSetup = void 0),
                                (this._licenseResponseCallback = void 0),
                                (this._emeEnabled = void 0),
                                (this._requestMediaKeySystemAccess = void 0),
                                (this._drmSystemOptions = void 0),
                                (this._config = void 0),
                                (this._mediaKeysList = []),
                                (this._media = null),
                                (this._hasSetMediaKeys = !1),
                                (this._requestLicenseFailureCount = 0),
                                (this.mediaKeysPromise = null),
                                (this._onMediaEncrypted = this.onMediaEncrypted.bind(this)),
                                (this.hls = t),
                                (this._config = t.config),
                                (this._widevineLicenseUrl = this._config.widevineLicenseUrl),
                                (this._licenseXhrSetup = this._config.licenseXhrSetup),
                                (this._licenseResponseCallback = this._config.licenseResponseCallback),
                                (this._emeEnabled = this._config.emeEnabled),
                                (this._requestMediaKeySystemAccess = this._config.requestMediaKeySystemAccessFunc),
                                (this._drmSystemOptions = this._config.drmSystemOptions),
                                this._registerListeners();
                        }
                        var e,
                            r,
                            i,
                            n = t.prototype;
                        return (
                            (n.destroy = function () {
                                this._unregisterListeners(), (this.hls = this._onMediaEncrypted = null), (this._requestMediaKeySystemAccess = null);
                            }),
                            (n._registerListeners = function () {
                                this.hls.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.on(a.a.MEDIA_DETACHED, this.onMediaDetached, this), this.hls.on(a.a.MANIFEST_PARSED, this.onManifestParsed, this);
                            }),
                            (n._unregisterListeners = function () {
                                this.hls.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.off(a.a.MEDIA_DETACHED, this.onMediaDetached, this), this.hls.off(a.a.MANIFEST_PARSED, this.onManifestParsed, this);
                            }),
                            (n.getLicenseServerUrl = function (t) {
                                switch (t) {
                                    case Er.WIDEVINE:
                                        if (!this._widevineLicenseUrl) break;
                                        return this._widevineLicenseUrl;
                                }
                                throw new Error('no license server URL configured for key-system "' + t + '"');
                            }),
                            (n._attemptKeySystemAccess = function (t, e, r) {
                                var i = this,
                                    n = (function (t, e, r, i) {
                                        switch (t) {
                                            case Er.WIDEVINE:
                                                return (function (t, e, r) {
                                                    var i = { audioCapabilities: [], videoCapabilities: [] };
                                                    return (
                                                        t.forEach(function (t) {
                                                            i.audioCapabilities.push({ contentType: 'audio/mp4; codecs="' + t + '"', robustness: r.audioRobustness || "" });
                                                        }),
                                                        e.forEach(function (t) {
                                                            i.videoCapabilities.push({ contentType: 'video/mp4; codecs="' + t + '"', robustness: r.videoRobustness || "" });
                                                        }),
                                                        [i]
                                                    );
                                                })(e, r, i);
                                            default:
                                                throw new Error("Unknown key-system: " + t);
                                        }
                                    })(t, e, r, this._drmSystemOptions);
                                o.b.log("Requesting encrypted media key-system access");
                                var a = this.requestMediaKeySystemAccess(t, n);
                                (this.mediaKeysPromise = a.then(function (e) {
                                    return i._onMediaKeySystemAccessObtained(t, e);
                                })),
                                    a.catch(function (e) {
                                        o.b.error('Failed to obtain key-system "' + t + '" access:', e);
                                    });
                            }),
                            (n._onMediaKeySystemAccessObtained = function (t, e) {
                                var r = this;
                                o.b.log('Access for key-system "' + t + '" obtained');
                                var i = { mediaKeysSessionInitialized: !1, mediaKeySystemAccess: e, mediaKeySystemDomain: t };
                                this._mediaKeysList.push(i);
                                var n = Promise.resolve()
                                    .then(function () {
                                        return e.createMediaKeys();
                                    })
                                    .then(function (e) {
                                        return (i.mediaKeys = e), o.b.log('Media-keys created for key-system "' + t + '"'), r._onMediaKeysCreated(), e;
                                    });
                                return (
                                    n.catch(function (t) {
                                        o.b.error("Failed to create media-keys:", t);
                                    }),
                                    n
                                );
                            }),
                            (n._onMediaKeysCreated = function () {
                                var t = this;
                                this._mediaKeysList.forEach(function (e) {
                                    e.mediaKeysSession || ((e.mediaKeysSession = e.mediaKeys.createSession()), t._onNewMediaKeySession(e.mediaKeysSession));
                                });
                            }),
                            (n._onNewMediaKeySession = function (t) {
                                var e = this;
                                o.b.log("New key-system session " + t.sessionId),
                                    t.addEventListener(
                                        "message",
                                        function (r) {
                                            e._onKeySessionMessage(t, r.message);
                                        },
                                        !1
                                    );
                            }),
                            (n._onKeySessionMessage = function (t, e) {
                                o.b.log("Got EME message event, creating license request"),
                                    this._requestLicense(e, function (e) {
                                        o.b.log("Received license data (length: " + (e ? e.byteLength : e) + "), updating key-session"), t.update(e);
                                    });
                            }),
                            (n.onMediaEncrypted = function (t) {
                                var e = this;
                                if ((o.b.log('Media is encrypted using "' + t.initDataType + '" init data type'), !this.mediaKeysPromise))
                                    return (
                                        o.b.error("Fatal: Media is encrypted but no CDM access or no keys have been requested"), void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_KEYS, fatal: !0 })
                                    );
                                var r = function (r) {
                                    e._media && (e._attemptSetMediaKeys(r), e._generateRequestWithPreferredKeySession(t.initDataType, t.initData));
                                };
                                this.mediaKeysPromise.then(r).catch(r);
                            }),
                            (n._attemptSetMediaKeys = function (t) {
                                if (!this._media) throw new Error("Attempted to set mediaKeys without first attaching a media element");
                                if (!this._hasSetMediaKeys) {
                                    var e = this._mediaKeysList[0];
                                    if (!e || !e.mediaKeys)
                                        return (
                                            o.b.error("Fatal: Media is encrypted but no CDM access or no keys have been obtained yet"),
                                            void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_KEYS, fatal: !0 })
                                        );
                                    o.b.log("Setting keys for encrypted media"), this._media.setMediaKeys(e.mediaKeys), (this._hasSetMediaKeys = !0);
                                }
                            }),
                            (n._generateRequestWithPreferredKeySession = function (t, e) {
                                var r = this,
                                    i = this._mediaKeysList[0];
                                if (!i)
                                    return (
                                        o.b.error("Fatal: Media is encrypted but not any key-system access has been obtained yet"),
                                        void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_ACCESS, fatal: !0 })
                                    );
                                if (i.mediaKeysSessionInitialized) o.b.warn("Key-Session already initialized but requested again");
                                else {
                                    var n = i.mediaKeysSession;
                                    if (!n) return o.b.error("Fatal: Media is encrypted but no key-session existing"), void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_SESSION, fatal: !0 });
                                    if (!e) return o.b.warn("Fatal: initData required for generating a key session is null"), void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_INIT_DATA, fatal: !0 });
                                    o.b.log('Generating key-session request for "' + t + '" init data type'),
                                        (i.mediaKeysSessionInitialized = !0),
                                        n
                                            .generateRequest(t, e)
                                            .then(function () {
                                                o.b.debug("Key-session generation succeeded");
                                            })
                                            .catch(function (t) {
                                                o.b.error("Error generating key-session request:", t), r.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_SESSION, fatal: !1 });
                                            });
                                }
                            }),
                            (n._createLicenseXhr = function (t, e, r) {
                                var i = new XMLHttpRequest();
                                (i.responseType = "arraybuffer"), (i.onreadystatechange = this._onLicenseRequestReadyStageChange.bind(this, i, t, e, r));
                                var n = this._licenseXhrSetup;
                                if (n)
                                    try {
                                        n.call(this.hls, i, t), (n = void 0);
                                    } catch (t) {
                                        o.b.error(t);
                                    }
                                try {
                                    i.readyState || i.open("POST", t, !0), n && n.call(this.hls, i, t);
                                } catch (t) {
                                    throw new Error("issue setting up KeySystem license XHR " + t);
                                }
                                return i;
                            }),
                            (n._onLicenseRequestReadyStageChange = function (t, e, r, i) {
                                switch (t.readyState) {
                                    case 4:
                                        if (200 === t.status) {
                                            (this._requestLicenseFailureCount = 0), o.b.log("License request succeeded");
                                            var n = t.response,
                                                l = this._licenseResponseCallback;
                                            if (l)
                                                try {
                                                    n = l.call(this.hls, t, e);
                                                } catch (t) {
                                                    o.b.error(t);
                                                }
                                            i(n);
                                        } else {
                                            if ((o.b.error("License Request XHR failed (" + e + "). Status: " + t.status + " (" + t.statusText + ")"), this._requestLicenseFailureCount++, this._requestLicenseFailureCount > 3))
                                                return void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_LICENSE_REQUEST_FAILED, fatal: !0 });
                                            var u = 3 - this._requestLicenseFailureCount + 1;
                                            o.b.warn("Retrying license request, " + u + " attempts left"), this._requestLicense(r, i);
                                        }
                                }
                            }),
                            (n._generateLicenseRequestChallenge = function (t, e) {
                                switch (t.mediaKeySystemDomain) {
                                    case Er.WIDEVINE:
                                        return e;
                                }
                                throw new Error("unsupported key-system: " + t.mediaKeySystemDomain);
                            }),
                            (n._requestLicense = function (t, e) {
                                o.b.log("Requesting content license for key-system");
                                var r = this._mediaKeysList[0];
                                if (!r)
                                    return (
                                        o.b.error("Fatal error: Media is encrypted but no key-system access has been obtained yet"),
                                        void this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_NO_ACCESS, fatal: !0 })
                                    );
                                try {
                                    var i = this.getLicenseServerUrl(r.mediaKeySystemDomain),
                                        n = this._createLicenseXhr(i, t, e);
                                    o.b.log("Sending license request to URL: " + i);
                                    var l = this._generateLicenseRequestChallenge(r, t);
                                    n.send(l);
                                } catch (t) {
                                    o.b.error("Failure requesting DRM license: " + t), this.hls.trigger(a.a.ERROR, { type: s.b.KEY_SYSTEM_ERROR, details: s.a.KEY_SYSTEM_LICENSE_REQUEST_FAILED, fatal: !0 });
                                }
                            }),
                            (n.onMediaAttached = function (t, e) {
                                if (this._emeEnabled) {
                                    var r = e.media;
                                    (this._media = r), r.addEventListener("encrypted", this._onMediaEncrypted);
                                }
                            }),
                            (n.onMediaDetached = function () {
                                var t = this._media,
                                    e = this._mediaKeysList;
                                t &&
                                    (t.removeEventListener("encrypted", this._onMediaEncrypted),
                                    (this._media = null),
                                    (this._mediaKeysList = []),
                                    Promise.all(
                                        e.map(function (t) {
                                            if (t.mediaKeysSession) return t.mediaKeysSession.close().catch(function () {});
                                        })
                                    )
                                        .then(function () {
                                            return t.setMediaKeys(null);
                                        })
                                        .catch(function () {}));
                            }),
                            (n.onManifestParsed = function (t, e) {
                                if (this._emeEnabled) {
                                    var r = e.levels
                                            .map(function (t) {
                                                return t.audioCodec;
                                            })
                                            .filter(function (t) {
                                                return !!t;
                                            }),
                                        i = e.levels
                                            .map(function (t) {
                                                return t.videoCodec;
                                            })
                                            .filter(function (t) {
                                                return !!t;
                                            });
                                    this._attemptKeySystemAccess(Er.WIDEVINE, r, i);
                                }
                            }),
                            (e = t),
                            (r = [
                                {
                                    key: "requestMediaKeySystemAccess",
                                    get: function () {
                                        if (!this._requestMediaKeySystemAccess) throw new Error("No requestMediaKeySystemAccess function configured");
                                        return this._requestMediaKeySystemAccess;
                                    },
                                },
                            ]) && Rr(e.prototype, r),
                            i && Rr(e, i),
                            t
                        );
                    })();
                function Cr(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                function wr(t, e, r) {
                    return e && Cr(t.prototype, e), r && Cr(t, r), t;
                }
                function Or(t, e) {
                    var r = ("undefined" != typeof Symbol && t[Symbol.iterator]) || t["@@iterator"];
                    if (r) return (r = r.call(t)).next.bind(r);
                    if (
                        Array.isArray(t) ||
                        (r = (function (t, e) {
                            if (!t) return;
                            if ("string" == typeof t) return xr(t, e);
                            var r = Object.prototype.toString.call(t).slice(8, -1);
                            "Object" === r && t.constructor && (r = t.constructor.name);
                            if ("Map" === r || "Set" === r) return Array.from(t);
                            if ("Arguments" === r || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(r)) return xr(t, e);
                        })(t)) ||
                        (e && t && "number" == typeof t.length)
                    ) {
                        r && (t = r);
                        var i = 0;
                        return function () {
                            return i >= t.length ? { done: !0 } : { done: !1, value: t[i++] };
                        };
                    }
                    throw new TypeError("Invalid attempt to iterate non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
                }
                function xr(t, e) {
                    (null == e || e > t.length) && (e = t.length);
                    for (var r = 0, i = new Array(e); r < e; r++) i[r] = t[r];
                    return i;
                }
                function Pr() {
                    return (Pr =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                !(function (t) {
                    (t.MANIFEST = "m"), (t.AUDIO = "a"), (t.VIDEO = "v"), (t.MUXED = "av"), (t.INIT = "i"), (t.CAPTION = "c"), (t.TIMED_TEXT = "tt"), (t.KEY = "k"), (t.OTHER = "o");
                })(Dr || (Dr = {})),
                    (function (t) {
                        (t.DASH = "d"), (t.HLS = "h"), (t.SMOOTH = "s"), (t.OTHER = "o");
                    })(kr || (kr = {})),
                    (function (t) {
                        (t.VOD = "v"), (t.LIVE = "l");
                    })(_r || (_r = {}));
                var Mr = (function () {
                        function t(e) {
                            var r = this;
                            (this.hls = void 0),
                                (this.config = void 0),
                                (this.media = void 0),
                                (this.sid = void 0),
                                (this.cid = void 0),
                                (this.useHeaders = !1),
                                (this.initialized = !1),
                                (this.starved = !1),
                                (this.buffering = !0),
                                (this.audioBuffer = void 0),
                                (this.videoBuffer = void 0),
                                (this.onWaiting = function () {
                                    r.initialized && (r.starved = !0), (r.buffering = !0);
                                }),
                                (this.onPlaying = function () {
                                    r.initialized || (r.initialized = !0), (r.buffering = !1);
                                }),
                                (this.applyPlaylistData = function (t) {
                                    try {
                                        r.apply(t, { ot: Dr.MANIFEST, su: !r.initialized });
                                    } catch (t) {
                                        o.b.warn("Could not generate manifest CMCD data.", t);
                                    }
                                }),
                                (this.applyFragmentData = function (t) {
                                    try {
                                        var e = t.frag,
                                            i = r.hls.levels[e.level],
                                            n = r.getObjectType(e),
                                            a = { d: 1e3 * e.duration, ot: n };
                                        (n !== Dr.VIDEO && n !== Dr.AUDIO && n != Dr.MUXED) || ((a.br = i.bitrate / 1e3), (a.tb = r.getTopBandwidth(n)), (a.bl = r.getBufferLength(n))), r.apply(t, a);
                                    } catch (t) {
                                        o.b.warn("Could not generate segment CMCD data.", t);
                                    }
                                }),
                                (this.hls = e);
                            var i = (this.config = e.config),
                                n = i.cmcd;
                            null != n &&
                                ((i.pLoader = this.createPlaylistLoader()),
                                (i.fLoader = this.createFragmentLoader()),
                                (this.sid = n.sessionId || t.uuid()),
                                (this.cid = n.contentId),
                                (this.useHeaders = !0 === n.useHeaders),
                                this.registerListeners());
                        }
                        var e = t.prototype;
                        return (
                            (e.registerListeners = function () {
                                var t = this.hls;
                                t.on(a.a.MEDIA_ATTACHED, this.onMediaAttached, this), t.on(a.a.MEDIA_DETACHED, this.onMediaDetached, this), t.on(a.a.BUFFER_CREATED, this.onBufferCreated, this);
                            }),
                            (e.unregisterListeners = function () {
                                var t = this.hls;
                                t.off(a.a.MEDIA_ATTACHED, this.onMediaAttached, this), t.off(a.a.MEDIA_DETACHED, this.onMediaDetached, this), t.off(a.a.BUFFER_CREATED, this.onBufferCreated, this), this.onMediaDetached();
                            }),
                            (e.destroy = function () {
                                this.unregisterListeners(), (this.hls = this.config = this.audioBuffer = this.videoBuffer = null);
                            }),
                            (e.onMediaAttached = function (t, e) {
                                (this.media = e.media), this.media.addEventListener("waiting", this.onWaiting), this.media.addEventListener("playing", this.onPlaying);
                            }),
                            (e.onMediaDetached = function () {
                                this.media && (this.media.removeEventListener("waiting", this.onWaiting), this.media.removeEventListener("playing", this.onPlaying), (this.media = null));
                            }),
                            (e.onBufferCreated = function (t, e) {
                                var r, i;
                                (this.audioBuffer = null === (r = e.tracks.audio) || void 0 === r ? void 0 : r.buffer), (this.videoBuffer = null === (i = e.tracks.video) || void 0 === i ? void 0 : i.buffer);
                            }),
                            (e.createData = function () {
                                var t;
                                return { v: 1, sf: kr.HLS, sid: this.sid, cid: this.cid, pr: null === (t = this.media) || void 0 === t ? void 0 : t.playbackRate, mtp: this.hls.bandwidthEstimate / 1e3 };
                            }),
                            (e.apply = function (e, r) {
                                void 0 === r && (r = {}), Pr(r, this.createData());
                                var i = r.ot === Dr.INIT || r.ot === Dr.VIDEO || r.ot === Dr.MUXED;
                                if ((this.starved && i && ((r.bs = !0), (r.su = !0), (this.starved = !1)), null == r.su && (r.su = this.buffering), this.useHeaders)) {
                                    var n = t.toHeaders(r);
                                    if (!Object.keys(n).length) return;
                                    e.headers || (e.headers = {}), Pr(e.headers, n);
                                } else {
                                    var a = t.toQuery(r);
                                    if (!a) return;
                                    e.url = t.appendQueryToUri(e.url, a);
                                }
                            }),
                            (e.getObjectType = function (t) {
                                var e = t.type;
                                return "subtitle" === e ? Dr.TIMED_TEXT : "initSegment" === t.sn ? Dr.INIT : "audio" === e ? Dr.AUDIO : "main" === e ? (this.hls.audioTracks.length ? Dr.VIDEO : Dr.MUXED) : void 0;
                            }),
                            (e.getTopBandwidth = function (t) {
                                for (var e, r = 0, i = Or(t === Dr.AUDIO ? this.hls.audioTracks : this.hls.levels); !(e = i()).done; ) {
                                    var n = e.value;
                                    n.bitrate > r && (r = n.bitrate);
                                }
                                return r > 0 ? r : NaN;
                            }),
                            (e.getBufferLength = function (t) {
                                var e = this.hls.media,
                                    r = t === Dr.AUDIO ? this.audioBuffer : this.videoBuffer;
                                return r && e ? 1e3 * lt.bufferInfo(r, e.currentTime, this.config.maxBufferHole).len : NaN;
                            }),
                            (e.createPlaylistLoader = function () {
                                var t = this.config.pLoader,
                                    e = this.applyPlaylistData,
                                    r = t || this.config.loader;
                                return (function () {
                                    function t(t) {
                                        (this.loader = void 0), (this.loader = new r(t));
                                    }
                                    var i = t.prototype;
                                    return (
                                        (i.destroy = function () {
                                            this.loader.destroy();
                                        }),
                                        (i.abort = function () {
                                            this.loader.abort();
                                        }),
                                        (i.load = function (t, r, i) {
                                            e(t), this.loader.load(t, r, i);
                                        }),
                                        wr(t, [
                                            {
                                                key: "stats",
                                                get: function () {
                                                    return this.loader.stats;
                                                },
                                            },
                                            {
                                                key: "context",
                                                get: function () {
                                                    return this.loader.context;
                                                },
                                            },
                                        ]),
                                        t
                                    );
                                })();
                            }),
                            (e.createFragmentLoader = function () {
                                var t = this.config.fLoader,
                                    e = this.applyFragmentData,
                                    r = t || this.config.loader;
                                return (function () {
                                    function t(t) {
                                        (this.loader = void 0), (this.loader = new r(t));
                                    }
                                    var i = t.prototype;
                                    return (
                                        (i.destroy = function () {
                                            this.loader.destroy();
                                        }),
                                        (i.abort = function () {
                                            this.loader.abort();
                                        }),
                                        (i.load = function (t, r, i) {
                                            e(t), this.loader.load(t, r, i);
                                        }),
                                        wr(t, [
                                            {
                                                key: "stats",
                                                get: function () {
                                                    return this.loader.stats;
                                                },
                                            },
                                            {
                                                key: "context",
                                                get: function () {
                                                    return this.loader.context;
                                                },
                                            },
                                        ]),
                                        t
                                    );
                                })();
                            }),
                            (t.uuid = function () {
                                var t = URL.createObjectURL(new Blob()),
                                    e = t.toString();
                                return URL.revokeObjectURL(t), e.substr(e.lastIndexOf("/") + 1);
                            }),
                            (t.serialize = function (t) {
                                for (
                                    var e,
                                        r = [],
                                        i = function (t) {
                                            return !Number.isNaN(t) && null != t && "" !== t && !1 !== t;
                                        },
                                        n = function (t) {
                                            return Math.round(t);
                                        },
                                        a = function (t) {
                                            return 100 * n(t / 100);
                                        },
                                        s = {
                                            br: n,
                                            d: n,
                                            bl: a,
                                            dl: a,
                                            mtp: a,
                                            nor: function (t) {
                                                return encodeURIComponent(t);
                                            },
                                            rtp: a,
                                            tb: n,
                                        },
                                        o = Or(Object.keys(t || {}).sort());
                                    !(e = o()).done;

                                ) {
                                    var l = e.value,
                                        u = t[l];
                                    if (i(u) && !(("v" === l && 1 === u) || ("pr" == l && 1 === u))) {
                                        var h = s[l];
                                        h && (u = h(u));
                                        var d = typeof u,
                                            c = void 0;
                                        (c = "ot" === l || "sf" === l || "st" === l ? l + "=" + u : "boolean" === d ? l : "number" === d ? l + "=" + u : l + "=" + JSON.stringify(u)), r.push(c);
                                    }
                                }
                                return r.join(",");
                            }),
                            (t.toHeaders = function (e) {
                                for (
                                    var r = {},
                                        i = ["Object", "Request", "Session", "Status"],
                                        n = [{}, {}, {}, {}],
                                        a = { br: 0, d: 0, ot: 0, tb: 0, bl: 1, dl: 1, mtp: 1, nor: 1, nrr: 1, su: 1, cid: 2, pr: 2, sf: 2, sid: 2, st: 2, v: 2, bs: 3, rtp: 3 },
                                        s = 0,
                                        o = Object.keys(e);
                                    s < o.length;
                                    s++
                                ) {
                                    var l = o[s];
                                    n[null != a[l] ? a[l] : 1][l] = e[l];
                                }
                                for (var u = 0; u < n.length; u++) {
                                    var h = t.serialize(n[u]);
                                    h && (r["CMCD-" + i[u]] = h);
                                }
                                return r;
                            }),
                            (t.toQuery = function (e) {
                                return "CMCD=" + encodeURIComponent(t.serialize(e));
                            }),
                            (t.appendQueryToUri = function (t, e) {
                                if (!e) return t;
                                var r = t.includes("?") ? "&" : "?";
                                return "" + t + r + e;
                            }),
                            t
                        );
                    })(),
                    Fr = r(12),
                    Nr = /^age:\s*[\d.]+\s*$/m,
                    Ur = (function () {
                        function t(t) {
                            (this.xhrSetup = void 0),
                                (this.requestTimeout = void 0),
                                (this.retryTimeout = void 0),
                                (this.retryDelay = void 0),
                                (this.config = null),
                                (this.callbacks = null),
                                (this.context = void 0),
                                (this.loader = null),
                                (this.stats = void 0),
                                (this.xhrSetup = t ? t.xhrSetup : null),
                                (this.stats = new Fr.a()),
                                (this.retryDelay = 0);
                        }
                        var e = t.prototype;
                        return (
                            (e.destroy = function () {
                                (this.callbacks = null), this.abortInternal(), (this.loader = null), (this.config = null);
                            }),
                            (e.abortInternal = function () {
                                var t = this.loader;
                                self.clearTimeout(this.requestTimeout), self.clearTimeout(this.retryTimeout), t && ((t.onreadystatechange = null), (t.onprogress = null), 4 !== t.readyState && ((this.stats.aborted = !0), t.abort()));
                            }),
                            (e.abort = function () {
                                var t;
                                this.abortInternal(), null !== (t = this.callbacks) && void 0 !== t && t.onAbort && this.callbacks.onAbort(this.stats, this.context, this.loader);
                            }),
                            (e.load = function (t, e, r) {
                                if (this.stats.loading.start) throw new Error("Loader can only be used once.");
                                (this.stats.loading.start = self.performance.now()), (this.context = t), (this.config = e), (this.callbacks = r), (this.retryDelay = e.retryDelay), this.loadInternal();
                            }),
                            (e.loadInternal = function () {
                                var t = this.config,
                                    e = this.context;
                                if (t) {
                                    var r = (this.loader = new self.XMLHttpRequest()),
                                        i = this.stats;
                                    (i.loading.first = 0), (i.loaded = 0);
                                    var n = this.xhrSetup;
                                    try {
                                        if (n)
                                            try {
                                                n(r, e.url);
                                            } catch (t) {
                                                r.open("GET", e.url, !0), n(r, e.url);
                                            }
                                        r.readyState || r.open("GET", e.url, !0);
                                        var a = this.context.headers;
                                        if (a) for (var s in a) r.setRequestHeader(s, a[s]);
                                    } catch (t) {
                                        return void this.callbacks.onError({ code: r.status, text: t.message }, e, r);
                                    }
                                    e.rangeEnd && r.setRequestHeader("Range", "bytes=" + e.rangeStart + "-" + (e.rangeEnd - 1)),
                                        (r.onreadystatechange = this.readystatechange.bind(this)),
                                        (r.onprogress = this.loadprogress.bind(this)),
                                        (r.responseType = e.responseType),
                                        self.clearTimeout(this.requestTimeout),
                                        (this.requestTimeout = self.setTimeout(this.loadtimeout.bind(this), t.timeout)),
                                        r.send();
                                }
                            }),
                            (e.readystatechange = function () {
                                var t = this.context,
                                    e = this.loader,
                                    r = this.stats;
                                if (t && e) {
                                    var i = e.readyState,
                                        n = this.config;
                                    if (!r.aborted && i >= 2)
                                        if ((self.clearTimeout(this.requestTimeout), 0 === r.loading.first && (r.loading.first = Math.max(self.performance.now(), r.loading.start)), 4 === i)) {
                                            (e.onreadystatechange = null), (e.onprogress = null);
                                            var a = e.status;
                                            if (a >= 200 && a < 300) {
                                                var s, l;
                                                if (
                                                    ((r.loading.end = Math.max(self.performance.now(), r.loading.first)),
                                                    (l = "arraybuffer" === t.responseType ? (s = e.response).byteLength : (s = e.responseText).length),
                                                    (r.loaded = r.total = l),
                                                    !this.callbacks)
                                                )
                                                    return;
                                                var u = this.callbacks.onProgress;
                                                if ((u && u(r, t, s, e), !this.callbacks)) return;
                                                var h = { url: e.responseURL, data: s };
                                                this.callbacks.onSuccess(h, r, t, e);
                                            } else
                                                r.retry >= n.maxRetry || (a >= 400 && a < 499)
                                                    ? (o.b.error(a + " while loading " + t.url), this.callbacks.onError({ code: a, text: e.statusText }, t, e))
                                                    : (o.b.warn(a + " while loading " + t.url + ", retrying in " + this.retryDelay + "..."),
                                                      this.abortInternal(),
                                                      (this.loader = null),
                                                      self.clearTimeout(this.retryTimeout),
                                                      (this.retryTimeout = self.setTimeout(this.loadInternal.bind(this), this.retryDelay)),
                                                      (this.retryDelay = Math.min(2 * this.retryDelay, n.maxRetryDelay)),
                                                      r.retry++);
                                        } else self.clearTimeout(this.requestTimeout), (this.requestTimeout = self.setTimeout(this.loadtimeout.bind(this), n.timeout));
                                }
                            }),
                            (e.loadtimeout = function () {
                                o.b.warn("timeout while loading " + this.context.url);
                                var t = this.callbacks;
                                t && (this.abortInternal(), t.onTimeout(this.stats, this.context, this.loader));
                            }),
                            (e.loadprogress = function (t) {
                                var e = this.stats;
                                (e.loaded = t.loaded), t.lengthComputable && (e.total = t.total);
                            }),
                            (e.getCacheAge = function () {
                                var t = null;
                                if (this.loader && Nr.test(this.loader.getAllResponseHeaders())) {
                                    var e = this.loader.getResponseHeader("age");
                                    t = e ? parseFloat(e) : null;
                                }
                                return t;
                            }),
                            t
                        );
                    })();
                function Br(t) {
                    var e = "function" == typeof Map ? new Map() : void 0;
                    return (Br = function (t) {
                        if (null === t || ((r = t), -1 === Function.toString.call(r).indexOf("[native code]"))) return t;
                        var r;
                        if ("function" != typeof t) throw new TypeError("Super expression must either be null or a function");
                        if (void 0 !== e) {
                            if (e.has(t)) return e.get(t);
                            e.set(t, i);
                        }
                        function i() {
                            return Gr(t, arguments, Hr(this).constructor);
                        }
                        return (i.prototype = Object.create(t.prototype, { constructor: { value: i, enumerable: !1, writable: !0, configurable: !0 } })), jr(i, t);
                    })(t);
                }
                function Gr(t, e, r) {
                    return (Gr = Kr()
                        ? Reflect.construct
                        : function (t, e, r) {
                              var i = [null];
                              i.push.apply(i, e);
                              var n = new (Function.bind.apply(t, i))();
                              return r && jr(n, r.prototype), n;
                          }).apply(null, arguments);
                }
                function Kr() {
                    if ("undefined" == typeof Reflect || !Reflect.construct) return !1;
                    if (Reflect.construct.sham) return !1;
                    if ("function" == typeof Proxy) return !0;
                    try {
                        return Boolean.prototype.valueOf.call(Reflect.construct(Boolean, [], function () {})), !0;
                    } catch (t) {
                        return !1;
                    }
                }
                function jr(t, e) {
                    return (jr =
                        Object.setPrototypeOf ||
                        function (t, e) {
                            return (t.__proto__ = e), t;
                        })(t, e);
                }
                function Hr(t) {
                    return (Hr = Object.setPrototypeOf
                        ? Object.getPrototypeOf
                        : function (t) {
                              return t.__proto__ || Object.getPrototypeOf(t);
                          })(t);
                }
                function Vr() {
                    return (Vr =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                var Wr = (function () {
                    function t(t) {
                        (this.fetchSetup = void 0),
                            (this.requestTimeout = void 0),
                            (this.request = void 0),
                            (this.response = void 0),
                            (this.controller = void 0),
                            (this.context = void 0),
                            (this.config = null),
                            (this.callbacks = null),
                            (this.stats = void 0),
                            (this.loader = null),
                            (this.fetchSetup = t.fetchSetup || Yr),
                            (this.controller = new self.AbortController()),
                            (this.stats = new Fr.a());
                    }
                    var e = t.prototype;
                    return (
                        (e.destroy = function () {
                            (this.loader = this.callbacks = null), this.abortInternal();
                        }),
                        (e.abortInternal = function () {
                            var t = this.response;
                            (t && t.ok) || ((this.stats.aborted = !0), this.controller.abort());
                        }),
                        (e.abort = function () {
                            var t;
                            this.abortInternal(), null !== (t = this.callbacks) && void 0 !== t && t.onAbort && this.callbacks.onAbort(this.stats, this.context, this.response);
                        }),
                        (e.load = function (t, e, r) {
                            var i = this,
                                a = this.stats;
                            if (a.loading.start) throw new Error("Loader can only be used once.");
                            a.loading.start = self.performance.now();
                            var s = (function (t, e) {
                                    var r = { method: "GET", mode: "cors", credentials: "same-origin", signal: e, headers: new self.Headers(Vr({}, t.headers)) };
                                    t.rangeEnd && r.headers.set("Range", "bytes=" + t.rangeStart + "-" + String(t.rangeEnd - 1));
                                    return r;
                                })(t, this.controller.signal),
                                o = r.onProgress,
                                l = "arraybuffer" === t.responseType,
                                u = l ? "byteLength" : "length";
                            (this.context = t),
                                (this.config = e),
                                (this.callbacks = r),
                                (this.request = this.fetchSetup(t, s)),
                                self.clearTimeout(this.requestTimeout),
                                (this.requestTimeout = self.setTimeout(function () {
                                    i.abortInternal(), r.onTimeout(a, t, i.response);
                                }, e.timeout)),
                                self
                                    .fetch(this.request)
                                    .then(function (r) {
                                        if (((i.response = i.loader = r), !r.ok)) {
                                            var s = r.status,
                                                u = r.statusText;
                                            throw new qr(u || "fetch, bad network response", s, r);
                                        }
                                        return (
                                            (a.loading.first = Math.max(self.performance.now(), a.loading.start)),
                                            (a.total = parseInt(r.headers.get("Content-Length") || "0")),
                                            o && Object(n.a)(e.highWaterMark) ? i.loadProgressively(r, a, t, e.highWaterMark, o) : l ? r.arrayBuffer() : r.text()
                                        );
                                    })
                                    .then(function (s) {
                                        var l = i.response;
                                        self.clearTimeout(i.requestTimeout), (a.loading.end = Math.max(self.performance.now(), a.loading.first)), (a.loaded = a.total = s[u]);
                                        var h = { url: l.url, data: s };
                                        o && !Object(n.a)(e.highWaterMark) && o(a, t, s, l), r.onSuccess(h, a, t, l);
                                    })
                                    .catch(function (e) {
                                        if ((self.clearTimeout(i.requestTimeout), !a.aborted)) {
                                            var n = e.code || 0;
                                            r.onError({ code: n, text: e.message }, t, e.details);
                                        }
                                    });
                        }),
                        (e.getCacheAge = function () {
                            var t = null;
                            if (this.response) {
                                var e = this.response.headers.get("age");
                                t = e ? parseFloat(e) : null;
                            }
                            return t;
                        }),
                        (e.loadProgressively = function (t, e, r, i, n) {
                            void 0 === i && (i = 0);
                            var a = new le.a(),
                                s = t.body.getReader();
                            return (function o() {
                                return s
                                    .read()
                                    .then(function (s) {
                                        if (s.done) return a.dataLength && n(e, r, a.flush(), t), Promise.resolve(new ArrayBuffer(0));
                                        var l = s.value,
                                            u = l.length;
                                        return (e.loaded += u), u < i || a.dataLength ? (a.push(l), a.dataLength >= i && n(e, r, a.flush(), t)) : n(e, r, l, t), o();
                                    })
                                    .catch(function () {
                                        return Promise.reject();
                                    });
                            })();
                        }),
                        t
                    );
                })();
                function Yr(t, e) {
                    return new self.Request(t.url, e);
                }
                var qr = (function (t) {
                        var e, r;
                        function i(e, r, i) {
                            var n;
                            return ((n = t.call(this, e) || this).code = void 0), (n.details = void 0), (n.code = r), (n.details = i), n;
                        }
                        return (r = t), ((e = i).prototype = Object.create(r.prototype)), (e.prototype.constructor = e), jr(e, r), i;
                    })(Br(Error)),
                    Xr = Wr,
                    zr = /\s/;
                function Qr() {
                    return (Qr =
                        Object.assign ||
                        function (t) {
                            for (var e = 1; e < arguments.length; e++) {
                                var r = arguments[e];
                                for (var i in r) Object.prototype.hasOwnProperty.call(r, i) && (t[i] = r[i]);
                            }
                            return t;
                        }).apply(this, arguments);
                }
                function $r(t, e) {
                    var r = Object.keys(t);
                    if (Object.getOwnPropertySymbols) {
                        var i = Object.getOwnPropertySymbols(t);
                        e &&
                            (i = i.filter(function (e) {
                                return Object.getOwnPropertyDescriptor(t, e).enumerable;
                            })),
                            r.push.apply(r, i);
                    }
                    return r;
                }
                function Jr(t) {
                    for (var e = 1; e < arguments.length; e++) {
                        var r = null != arguments[e] ? arguments[e] : {};
                        e % 2
                            ? $r(Object(r), !0).forEach(function (e) {
                                  Zr(t, e, r[e]);
                              })
                            : Object.getOwnPropertyDescriptors
                            ? Object.defineProperties(t, Object.getOwnPropertyDescriptors(r))
                            : $r(Object(r)).forEach(function (e) {
                                  Object.defineProperty(t, e, Object.getOwnPropertyDescriptor(r, e));
                              });
                    }
                    return t;
                }
                function Zr(t, e, r) {
                    return e in t ? Object.defineProperty(t, e, { value: r, enumerable: !0, configurable: !0, writable: !0 }) : (t[e] = r), t;
                }
                var ti = Jr(
                    Jr(
                        {
                            autoStartLoad: !0,
                            startPosition: -1,
                            defaultAudioCodec: void 0,
                            debug: !1,
                            capLevelOnFPSDrop: !1,
                            capLevelToPlayerSize: !1,
                            initialLiveManifestSize: 1,
                            maxBufferLength: 30,
                            backBufferLength: 1 / 0,
                            maxBufferSize: 6e7,
                            maxBufferHole: 0.1,
                            highBufferWatchdogPeriod: 2,
                            nudgeOffset: 0.1,
                            nudgeMaxRetry: 3,
                            maxFragLookUpTolerance: 0.25,
                            liveSyncDurationCount: 3,
                            liveMaxLatencyDurationCount: 1 / 0,
                            liveSyncDuration: void 0,
                            liveMaxLatencyDuration: void 0,
                            maxLiveSyncPlaybackRate: 1,
                            liveDurationInfinity: !1,
                            liveBackBufferLength: null,
                            maxMaxBufferLength: 600,
                            enableWorker: !0,
                            enableSoftwareAES: !0,
                            manifestLoadingTimeOut: 1e4,
                            manifestLoadingMaxRetry: 1,
                            manifestLoadingRetryDelay: 1e3,
                            manifestLoadingMaxRetryTimeout: 64e3,
                            startLevel: void 0,
                            levelLoadingTimeOut: 1e4,
                            levelLoadingMaxRetry: 4,
                            levelLoadingRetryDelay: 1e3,
                            levelLoadingMaxRetryTimeout: 64e3,
                            fragLoadingTimeOut: 2e4,
                            fragLoadingMaxRetry: 6,
                            fragLoadingRetryDelay: 1e3,
                            fragLoadingMaxRetryTimeout: 64e3,
                            startFragPrefetch: !1,
                            fpsDroppedMonitoringPeriod: 5e3,
                            fpsDroppedMonitoringThreshold: 0.2,
                            appendErrorMaxRetry: 3,
                            loader: Ur,
                            fLoader: void 0,
                            pLoader: void 0,
                            xhrSetup: void 0,
                            licenseXhrSetup: void 0,
                            licenseResponseCallback: void 0,
                            abrController: oe,
                            bufferController: De,
                            capLevelController: Sr,
                            fpsController: Lr,
                            stretchShortVideoTrack: !1,
                            maxAudioFramesDrift: 1,
                            forceKeyFrameOnDiscontinuity: !0,
                            abrEwmaFastLive: 3,
                            abrEwmaSlowLive: 9,
                            abrEwmaFastVoD: 3,
                            abrEwmaSlowVoD: 9,
                            abrEwmaDefaultEstimate: 5e5,
                            abrBandWidthFactor: 0.95,
                            abrBandWidthUpFactor: 0.7,
                            abrMaxWithRealBitrate: !1,
                            maxStarvationDelay: 4,
                            maxLoadingDelay: 4,
                            minAutoBitrate: 0,
                            emeEnabled: !1,
                            widevineLicenseUrl: void 0,
                            drmSystemOptions: {},
                            requestMediaKeySystemAccessFunc: Ar,
                            testBandwidth: !0,
                            progressive: !1,
                            lowLatencyMode: !0,
                            cmcd: void 0,
                        },
                        {
                            cueHandler: {
                                newCue: function (t, e, r, i) {
                                    for (var n, a, s, o, l, u = [], h = self.VTTCue || self.TextTrackCue, d = 0; d < i.rows.length; d++)
                                        if (((s = !0), (o = 0), (l = ""), !(n = i.rows[d]).isEmpty())) {
                                            for (var c = 0; c < n.chars.length; c++) zr.test(n.chars[c].uchar) && s ? o++ : ((l += n.chars[c].uchar), (s = !1));
                                            (n.cueStartTime = e), e === r && (r += 1e-4), o >= 16 ? o-- : o++;
                                            var f = Ze(l.trim()),
                                                g = sr(e, r, f);
                                            (t && t.cues && t.cues.getCueById(g)) || (((a = new h(e, r, f)).id = g), (a.line = d + 1), (a.align = "left"), (a.position = 10 + Math.min(80, 10 * Math.floor((8 * o) / 32))), u.push(a));
                                        }
                                    return (
                                        t &&
                                            u.length &&
                                            (u.sort(function (t, e) {
                                                return "auto" === t.line || "auto" === e.line ? 0 : t.line > 8 && e.line > 8 ? e.line - t.line : t.line - e.line;
                                            }),
                                            u.forEach(function (e) {
                                                return O(t, e);
                                            })),
                                        u
                                    );
                                },
                            },
                            enableCEA708Captions: !0,
                            enableWebVTT: !0,
                            enableIMSC1: !0,
                            captionsTextTrack1Label: "English",
                            captionsTextTrack1LanguageCode: "en",
                            captionsTextTrack2Label: "Spanish",
                            captionsTextTrack2LanguageCode: "es",
                            captionsTextTrack3Label: "Unknown CC",
                            captionsTextTrack3LanguageCode: "",
                            captionsTextTrack4Label: "Unknown CC",
                            captionsTextTrack4LanguageCode: "",
                            renderTextTracksNatively: !0,
                        }
                    ),
                    {},
                    { subtitleStreamController: me, subtitleTrackController: Se, timelineController: yr, audioStreamController: de, audioTrackController: ge, emeController: Ir, cmcdController: Mr }
                );
                function ei(t) {
                    var e = t.loader;
                    e !== Xr && e !== Ur
                        ? (o.b.log("[config]: Custom loader detected, cannot enable progressive streaming"), (t.progressive = !1))
                        : (function () {
                              if (self.fetch && self.AbortController && self.ReadableStream && self.Request)
                                  try {
                                      return new self.ReadableStream({}), !0;
                                  } catch (t) {}
                              return !1;
                          })() && ((t.loader = Xr), (t.progressive = !0), (t.enableSoftwareAES = !0), o.b.log("[config]: Progressive streaming enabled, using FetchLoader"));
                }
                function ri(t, e) {
                    for (var r = 0; r < e.length; r++) {
                        var i = e[r];
                        (i.enumerable = i.enumerable || !1), (i.configurable = !0), "value" in i && (i.writable = !0), Object.defineProperty(t, i.key, i);
                    }
                }
                var ii = (function () {
                    function t(e) {
                        void 0 === e && (e = {}),
                            (this.config = void 0),
                            (this.userConfig = void 0),
                            (this.coreComponents = void 0),
                            (this.networkControllers = void 0),
                            (this._emitter = new $t.EventEmitter()),
                            (this._autoLevelCapping = void 0),
                            (this.abrController = void 0),
                            (this.bufferController = void 0),
                            (this.capLevelController = void 0),
                            (this.latencyController = void 0),
                            (this.levelController = void 0),
                            (this.streamController = void 0),
                            (this.audioTrackController = void 0),
                            (this.subtitleTrackController = void 0),
                            (this.emeController = void 0),
                            (this.cmcdController = void 0),
                            (this._media = null),
                            (this.url = null);
                        var r = (this.config = (function (t, e) {
                            if ((e.liveSyncDurationCount || e.liveMaxLatencyDurationCount) && (e.liveSyncDuration || e.liveMaxLatencyDuration))
                                throw new Error("Illegal hls.js config: don't mix up liveSyncDurationCount/liveMaxLatencyDurationCount and liveSyncDuration/liveMaxLatencyDuration");
                            if (void 0 !== e.liveMaxLatencyDurationCount && (void 0 === e.liveSyncDurationCount || e.liveMaxLatencyDurationCount <= e.liveSyncDurationCount))
                                throw new Error('Illegal hls.js config: "liveMaxLatencyDurationCount" must be greater than "liveSyncDurationCount"');
                            if (void 0 !== e.liveMaxLatencyDuration && (void 0 === e.liveSyncDuration || e.liveMaxLatencyDuration <= e.liveSyncDuration))
                                throw new Error('Illegal hls.js config: "liveMaxLatencyDuration" must be greater than "liveSyncDuration"');
                            return Qr({}, t, e);
                        })(t.DefaultConfig, e));
                        (this.userConfig = e), Object(o.a)(r.debug), (this._autoLevelCapping = -1), r.progressive && ei(r);
                        var i = r.abrController,
                            n = r.bufferController,
                            a = r.capLevelController,
                            s = r.fpsController,
                            l = (this.abrController = new i(this)),
                            u = (this.bufferController = new n(this)),
                            h = (this.capLevelController = new a(this)),
                            d = new s(this),
                            c = new I(this),
                            f = new C(this),
                            g = new F(this),
                            v = (this.levelController = new rt(this)),
                            p = new it(this),
                            m = (this.streamController = new ie(this, p));
                        h.setStreamController(m), d.setStreamController(m);
                        var y = [v, m];
                        this.networkControllers = y;
                        var T = [c, f, l, u, h, d, g, p];
                        (this.audioTrackController = this.createController(r.audioTrackController, null, y)),
                            this.createController(r.audioStreamController, p, y),
                            (this.subtitleTrackController = this.createController(r.subtitleTrackController, null, y)),
                            this.createController(r.subtitleStreamController, p, y),
                            this.createController(r.timelineController, null, T),
                            (this.emeController = this.createController(r.emeController, null, T)),
                            (this.cmcdController = this.createController(r.cmcdController, null, T)),
                            (this.latencyController = this.createController(B, null, T)),
                            (this.coreComponents = T);
                    }
                    t.isSupported = function () {
                        return (function () {
                            var t = qt();
                            if (!t) return !1;
                            var e = Xt(),
                                r = t && "function" == typeof t.isTypeSupported && t.isTypeSupported('video/mp4; codecs="avc1.42E01E,mp4a.40.2"'),
                                i = !e || (e.prototype && "function" == typeof e.prototype.appendBuffer && "function" == typeof e.prototype.remove);
                            return !!r && !!i;
                        })();
                    };
                    var e,
                        r,
                        n,
                        l = t.prototype;
                    return (
                        (l.createController = function (t, e, r) {
                            if (t) {
                                var i = e ? new t(this, e) : new t(this);
                                return r && r.push(i), i;
                            }
                            return null;
                        }),
                        (l.on = function (t, e, r) {
                            void 0 === r && (r = this), this._emitter.on(t, e, r);
                        }),
                        (l.once = function (t, e, r) {
                            void 0 === r && (r = this), this._emitter.once(t, e, r);
                        }),
                        (l.removeAllListeners = function (t) {
                            this._emitter.removeAllListeners(t);
                        }),
                        (l.off = function (t, e, r, i) {
                            void 0 === r && (r = this), this._emitter.off(t, e, r, i);
                        }),
                        (l.listeners = function (t) {
                            return this._emitter.listeners(t);
                        }),
                        (l.emit = function (t, e, r) {
                            return this._emitter.emit(t, e, r);
                        }),
                        (l.trigger = function (t, e) {
                            if (this.config.debug) return this.emit(t, t, e);
                            try {
                                return this.emit(t, t, e);
                            } catch (e) {
                                o.b.error("An internal error happened while handling event " + t + '. Error message: "' + e.message + '". Here is a stacktrace:', e),
                                    this.trigger(a.a.ERROR, { type: s.b.OTHER_ERROR, details: s.a.INTERNAL_EXCEPTION, fatal: !1, event: t, error: e });
                            }
                            return !1;
                        }),
                        (l.listenerCount = function (t) {
                            return this._emitter.listenerCount(t);
                        }),
                        (l.destroy = function () {
                            o.b.log("destroy"),
                                this.trigger(a.a.DESTROYING, void 0),
                                this.detachMedia(),
                                this.removeAllListeners(),
                                (this._autoLevelCapping = -1),
                                (this.url = null),
                                this.networkControllers.forEach(function (t) {
                                    return t.destroy();
                                }),
                                (this.networkControllers.length = 0),
                                this.coreComponents.forEach(function (t) {
                                    return t.destroy();
                                }),
                                (this.coreComponents.length = 0);
                        }),
                        (l.attachMedia = function (t) {
                            o.b.log("attachMedia"), (this._media = t), this.trigger(a.a.MEDIA_ATTACHING, { media: t });
                        }),
                        (l.detachMedia = function () {
                            o.b.log("detachMedia"), this.trigger(a.a.MEDIA_DETACHING, void 0), (this._media = null);
                        }),
                        (l.loadSource = function (t) {
                            this.stopLoad();
                            var e = this.media,
                                r = this.url,
                                n = (this.url = i.buildAbsoluteURL(self.location.href, t, { alwaysNormalize: !0 }));
                            o.b.log("loadSource:" + n), e && r && r !== n && this.bufferController.hasSourceTypes() && (this.detachMedia(), this.attachMedia(e)), this.trigger(a.a.MANIFEST_LOADING, { url: t });
                        }),
                        (l.startLoad = function (t) {
                            void 0 === t && (t = -1),
                                o.b.log("startLoad(" + t + ")"),
                                this.networkControllers.forEach(function (e) {
                                    e.startLoad(t);
                                });
                        }),
                        (l.stopLoad = function () {
                            o.b.log("stopLoad"),
                                this.networkControllers.forEach(function (t) {
                                    t.stopLoad();
                                });
                        }),
                        (l.swapAudioCodec = function () {
                            o.b.log("swapAudioCodec"), this.streamController.swapAudioCodec();
                        }),
                        (l.recoverMediaError = function () {
                            o.b.log("recoverMediaError");
                            var t = this._media;
                            this.detachMedia(), t && this.attachMedia(t);
                        }),
                        (l.removeLevel = function (t, e) {
                            void 0 === e && (e = 0), this.levelController.removeLevel(t, e);
                        }),
                        (e = t),
                        (n = [
                            {
                                key: "version",
                                get: function () {
                                    return "1.1.1";
                                },
                            },
                            {
                                key: "Events",
                                get: function () {
                                    return a.a;
                                },
                            },
                            {
                                key: "ErrorTypes",
                                get: function () {
                                    return s.b;
                                },
                            },
                            {
                                key: "ErrorDetails",
                                get: function () {
                                    return s.a;
                                },
                            },
                            {
                                key: "DefaultConfig",
                                get: function () {
                                    return t.defaultConfig ? t.defaultConfig : ti;
                                },
                                set: function (e) {
                                    t.defaultConfig = e;
                                },
                            },
                        ]),
                        (r = [
                            {
                                key: "levels",
                                get: function () {
                                    var t = this.levelController.levels;
                                    return t || [];
                                },
                            },
                            {
                                key: "currentLevel",
                                get: function () {
                                    return this.streamController.currentLevel;
                                },
                                set: function (t) {
                                    o.b.log("set currentLevel:" + t), (this.loadLevel = t), this.abrController.clearTimer(), this.streamController.immediateLevelSwitch();
                                },
                            },
                            {
                                key: "nextLevel",
                                get: function () {
                                    return this.streamController.nextLevel;
                                },
                                set: function (t) {
                                    o.b.log("set nextLevel:" + t), (this.levelController.manualLevel = t), this.streamController.nextLevelSwitch();
                                },
                            },
                            {
                                key: "loadLevel",
                                get: function () {
                                    return this.levelController.level;
                                },
                                set: function (t) {
                                    o.b.log("set loadLevel:" + t), (this.levelController.manualLevel = t);
                                },
                            },
                            {
                                key: "nextLoadLevel",
                                get: function () {
                                    return this.levelController.nextLoadLevel;
                                },
                                set: function (t) {
                                    this.levelController.nextLoadLevel = t;
                                },
                            },
                            {
                                key: "firstLevel",
                                get: function () {
                                    return Math.max(this.levelController.firstLevel, this.minAutoLevel);
                                },
                                set: function (t) {
                                    o.b.log("set firstLevel:" + t), (this.levelController.firstLevel = t);
                                },
                            },
                            {
                                key: "startLevel",
                                get: function () {
                                    return this.levelController.startLevel;
                                },
                                set: function (t) {
                                    o.b.log("set startLevel:" + t), -1 !== t && (t = Math.max(t, this.minAutoLevel)), (this.levelController.startLevel = t);
                                },
                            },
                            {
                                key: "capLevelToPlayerSize",
                                get: function () {
                                    return this.config.capLevelToPlayerSize;
                                },
                                set: function (t) {
                                    var e = !!t;
                                    e !== this.config.capLevelToPlayerSize &&
                                        (e ? this.capLevelController.startCapping() : (this.capLevelController.stopCapping(), (this.autoLevelCapping = -1), this.streamController.nextLevelSwitch()), (this.config.capLevelToPlayerSize = e));
                                },
                            },
                            {
                                key: "autoLevelCapping",
                                get: function () {
                                    return this._autoLevelCapping;
                                },
                                set: function (t) {
                                    this._autoLevelCapping !== t && (o.b.log("set autoLevelCapping:" + t), (this._autoLevelCapping = t));
                                },
                            },
                            {
                                key: "bandwidthEstimate",
                                get: function () {
                                    var t = this.abrController.bwEstimator;
                                    return t ? t.getEstimate() : NaN;
                                },
                            },
                            {
                                key: "autoLevelEnabled",
                                get: function () {
                                    return -1 === this.levelController.manualLevel;
                                },
                            },
                            {
                                key: "manualLevel",
                                get: function () {
                                    return this.levelController.manualLevel;
                                },
                            },
                            {
                                key: "minAutoLevel",
                                get: function () {
                                    var t = this.levels,
                                        e = this.config.minAutoBitrate;
                                    if (!t) return 0;
                                    for (var r = t.length, i = 0; i < r; i++) if (t[i].maxBitrate > e) return i;
                                    return 0;
                                },
                            },
                            {
                                key: "maxAutoLevel",
                                get: function () {
                                    var t = this.levels,
                                        e = this.autoLevelCapping;
                                    return -1 === e && t && t.length ? t.length - 1 : e;
                                },
                            },
                            {
                                key: "nextAutoLevel",
                                get: function () {
                                    return Math.min(Math.max(this.abrController.nextAutoLevel, this.minAutoLevel), this.maxAutoLevel);
                                },
                                set: function (t) {
                                    this.abrController.nextAutoLevel = Math.max(this.minAutoLevel, t);
                                },
                            },
                            {
                                key: "audioTracks",
                                get: function () {
                                    var t = this.audioTrackController;
                                    return t ? t.audioTracks : [];
                                },
                            },
                            {
                                key: "audioTrack",
                                get: function () {
                                    var t = this.audioTrackController;
                                    return t ? t.audioTrack : -1;
                                },
                                set: function (t) {
                                    var e = this.audioTrackController;
                                    e && (e.audioTrack = t);
                                },
                            },
                            {
                                key: "subtitleTracks",
                                get: function () {
                                    var t = this.subtitleTrackController;
                                    return t ? t.subtitleTracks : [];
                                },
                            },
                            {
                                key: "subtitleTrack",
                                get: function () {
                                    var t = this.subtitleTrackController;
                                    return t ? t.subtitleTrack : -1;
                                },
                                set: function (t) {
                                    var e = this.subtitleTrackController;
                                    e && (e.subtitleTrack = t);
                                },
                            },
                            {
                                key: "media",
                                get: function () {
                                    return this._media;
                                },
                            },
                            {
                                key: "subtitleDisplay",
                                get: function () {
                                    var t = this.subtitleTrackController;
                                    return !!t && t.subtitleDisplay;
                                },
                                set: function (t) {
                                    var e = this.subtitleTrackController;
                                    e && (e.subtitleDisplay = t);
                                },
                            },
                            {
                                key: "lowLatencyMode",
                                get: function () {
                                    return this.config.lowLatencyMode;
                                },
                                set: function (t) {
                                    this.config.lowLatencyMode = t;
                                },
                            },
                            {
                                key: "liveSyncPosition",
                                get: function () {
                                    return this.latencyController.liveSyncPosition;
                                },
                            },
                            {
                                key: "latency",
                                get: function () {
                                    return this.latencyController.latency;
                                },
                            },
                            {
                                key: "maxLatency",
                                get: function () {
                                    return this.latencyController.maxLatency;
                                },
                            },
                            {
                                key: "targetLatency",
                                get: function () {
                                    return this.latencyController.targetLatency;
                                },
                            },
                            {
                                key: "drift",
                                get: function () {
                                    return this.latencyController.drift;
                                },
                            },
                            {
                                key: "forceStartLoad",
                                get: function () {
                                    return this.streamController.forceStartLoad;
                                },
                            },
                        ]) && ri(e.prototype, r),
                        n && ri(e, n),
                        t
                    );
                })();
                ii.defaultConfig = void 0;
            },
        ]).default;
    });
//# sourceMappingURL=hls.min.js.map
