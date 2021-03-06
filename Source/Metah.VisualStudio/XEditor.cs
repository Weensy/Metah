﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Language.StandardClassification;
using Metah.Compilation;
using Metah.MSBuild;

namespace Metah.VisualStudio.Editors.X {
    internal static class ContentTypeDefinitions {
        internal const string XContentType = "MetahX";
        internal const string XFileExtension = ".mx";
        [Export, BaseDefinition("code"), Name(XContentType)]
        internal static ContentTypeDefinition XContentTypeDefinition = null;
        [Export, ContentType(XContentType), FileExtension(XFileExtension)]
        internal static FileExtensionToContentTypeDefinition XFileExtensionDefinition = null;
        //
        internal const string XCSharpContentType = "MetahXCSharp";
        internal const string XCSharpFileExtension = ".mxcs";
        [Export, BaseDefinition("code"), Name(XCSharpContentType)]
        internal static ContentTypeDefinition XCSharpContentTypeDefinition = null;
        [Export, ContentType(XCSharpContentType), FileExtension(XCSharpFileExtension)]
        internal static FileExtensionToContentTypeDefinition XCSharpFileExtensionDefinition = null;
    }

    [Export(typeof(IClassifierProvider)), ContentType(ContentTypeDefinitions.XContentType), ContentType(ContentTypeDefinitions.XCSharpContentType)]
    internal sealed class LanguageClassifierProvider : IClassifierProvider {
        [Import]
        internal IStandardClassificationService StandardService = null;
        [Import]
        internal IClassificationTypeRegistryService RegistryService = null;
        public IClassifier GetClassifier(ITextBuffer textBuffer) {
            return textBuffer.Properties.GetOrCreateSingletonProperty<LanguageClassifier>(() => new LanguageClassifier(textBuffer, StandardService, RegistryService));
        }
    }
    internal sealed class LanguageClassifier : LanguageClassifierBase {
        internal LanguageClassifier(ITextBuffer textBuffer, IStandardClassificationService standardService, IClassificationTypeRegistryService registryService)
            : base(textBuffer, standardService, registryService, _keywordSet) {
        }
        private static readonly HashSet<string> _keywordSet = XTokens.KeywordSet;
    }
    [Export(typeof(ITaggerProvider)), TagType(typeof(IErrorTag)), ContentType(ContentTypeDefinitions.XContentType), ContentType(ContentTypeDefinitions.XCSharpContentType)]
    internal sealed class LanguageErrorTaggerProvider : LanguageErrorTaggerProviderBase {
        internal LanguageErrorTaggerProvider() : base(XBuildErrorStore.FileName, XBuildErrorStore.TryLoad) { }
    }

}
